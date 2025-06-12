using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using FishyLibrary.Models;
using Npgsql;

namespace FishyLibrary.Utils;

/*
 * Used as a utils to reduce code duplication for data access classes
 */
public class DataAccess
{
    public static string GetConnectionString(string environment)
    {
        string host = "";
        int port = 0;
        string database = "";
        if (environment == "Development")
        {
            host = "localhost";
            port = 1;
            database = "dev-1";
        }
        else if (environment == "Production")
        {
            host = "db";
            port = 1;
            database = "1";
        }
        else if (environment == "Staging")
        {
            host = "127.0.0.1";
            port = 4001;
            database = "1";
        }
        else
        {
            host = "localhost";
            port = 4321;
            database = "test-1";
        }

        return $"Host={host};Port={port};Database={database};User Id=postgres;Password=1;";
    }
    
    private static string FormatInsertValues(List<string> values)
    {
        string result = " (";
        foreach (var value in values)
        {
            result += value + ",";
        }

        result = result.Remove(result.Length - 1, 1);
        result += ") ";
        return result;
    }

    /*
     * Gets the property info for a given object and property name
     */
    public static PropertyInfo GetPropertyInfo(Type obj, string property)
    {
        PropertyInfo? propertyInfo = obj.GetProperty(property);
        if (propertyInfo is not null)
        {
            return propertyInfo;
        }

        throw new Exception("Property Not found " + property);
    }

    /*
     * Loads all data of a given type into a list
     */
    public static List<T> LoadAllDataInternal<T>(string sqlCommand, Action<NpgsqlDataReader, List<T>> loadHandler, string connectionString)
    {
        List<T> data = new List<T>();

        try
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using NpgsqlCommand command = new NpgsqlCommand(sqlCommand, connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            loadHandler(reader, data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return data;
    }

    /*
     * Executes the sql command and returns true if more than 1 row is changed
     */
    private static bool InsertDataInternal(string sqlCommand, string connectionString)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(sqlCommand, connection);
            var result = command.ExecuteNonQuery();
            return result != 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    /*
     * Executes a non-query SQL command (e.g., DELETE, UPDATE, INSERT)
     */
    public static int ExecuteNonQueryInternal(string sqlCommand, string connectionString)
    {
        try
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            using NpgsqlCommand command = new NpgsqlCommand(sqlCommand, connection);
            return command.ExecuteNonQuery(); // Returns the number of affected rows
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1; // Return a negative value in case of error
        }
    }


    /*
     * Inserts a bulk list of data into the given table
     *  @table - table name in camel case
     *  @data - list of data objects to write to table
     *  @properties - list of pairs where key is the column name and the property info is the corresponding property
     */
    public static bool InsertBulk<T>(string table, List<T> data, List<KeyValuePair<string, PropertyInfo>> properties, string connectionString)
    {
        // Start building the SQL command
        string sqlCommand = "INSERT INTO " + table +
                            FormatInsertValues(properties.Select(p => p.Key).ToList()) + " VALUES ";

        // Process each row of data
        foreach (var row in data)
        {
            sqlCommand += "("; // Start of value group

            // Format each property value correctly based on its type
            sqlCommand += string.Join(",", properties.Select(p =>
            {
                var value = p.Value.GetValue(row);

                // Check for null values
                if (value == null)
                {
                    return "NULL";
                }

                // Handle different types of properties
                if (p.Value.PropertyType == typeof(string))
                {
                    return $"'{value}'"; // Quote string values
                }

                if (p.Value.PropertyType == typeof(DateTime))
                {
                    // Format DateTime to 'yyyy-MM-dd HH:mm:ss' for PostgreSQL
                    return $"'{((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss")}'";
                }

                if (p.Value.PropertyType == typeof(DateOnly))
                {
                    // Format DateOnly to 'yyyy-MM-dd'
                    return $"'{((DateOnly)value).ToString("yyyy-MM-dd")}'";
                }

                if (p.Value.PropertyType == typeof(decimal) || p.Value.PropertyType == typeof(double))
                {
                    // Ensure decimal/double values are formatted with dot for PostgreSQL
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                }

                // For all other types (like int), just call ToString()
                return value.ToString();
            }));

            sqlCommand += "),"; // End of value group
        }

        // Remove the trailing comma at the end of the SQL command
        sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);

        // Execute the SQL command
        return InsertDataInternal(sqlCommand, connectionString);
    }
}