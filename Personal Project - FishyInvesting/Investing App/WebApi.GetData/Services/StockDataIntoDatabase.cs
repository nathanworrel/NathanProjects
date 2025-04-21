using System.Reflection;
using FishyLibrary.Models;
using Npgsql;

namespace FishyLibrary.Utils;

public class StockDataIntoDatabase
{
    private const string PRICING_DATA = "pricing_data";
    private const string PRICE = "price";
    private const string PRODUCT = "product";
    private const string TIME = "time";
    private string connectionString = "";

    public StockDataIntoDatabase(string connectionString)
    {
        this.connectionString = connectionString;
    }

    /*
     * Inserts a list of StockData
     */
    public bool InsertAllData(List<StockData> data)
    {
        Dictionary<string, PropertyInfo> propertyInfos = new Dictionary<string, PropertyInfo>();
        propertyInfos.Add(PRODUCT, DataAccess.GetPropertyInfo(typeof(StockData), nameof(StockData.Product)));
        propertyInfos.Add(TIME, DataAccess.GetPropertyInfo(typeof(StockData), nameof(StockData.Time)));
        propertyInfos.Add(PRICE, DataAccess.GetPropertyInfo(typeof(StockData), nameof(StockData.Price)));

        return DataAccess.InsertBulk(PRICING_DATA, data, propertyInfos.ToList(), connectionString);
    }

    public StockData? LoadRecentData(string product)
    {
        return DataAccess.LoadAllDataInternal<StockData>(
            "SELECT * FROM " + PRICING_DATA + " where " + PRODUCT + " = '" + product + "' ORDER BY " + TIME +
            " desc limit 1;", LoadHandler, connectionString).FirstOrDefault();
    }

    public List<StockData> LoadDataAtTime(string product, DateTime time, DateTime startDate)
    {
        string sql =
            $"SELECT * FROM {PRICING_DATA} WHERE {PRODUCT} = '{product}' AND {TIME} >= '{startDate:yyyy-MM-dd HH:mm:ss}' AND EXTRACT(HOUR FROM {TIME}) = {time.Hour} and EXTRACT(MINUTE  FROM {TIME}) = {time.Minute} ORDER BY {TIME} asc;";
        return DataAccess.LoadAllDataInternal<StockData>(sql, LoadHandler, connectionString);
    }
    
    public List<StockData> LoadData(string product)
    {
        string sql =
            $"SELECT * FROM {PRICING_DATA} WHERE {PRODUCT} = '{product}' ORDER BY {TIME} asc;";
        return DataAccess.LoadAllDataInternal<StockData>(sql, LoadHandler, connectionString);
    }


    private static void LoadHandler(NpgsqlDataReader reader, List<StockData> data)
    {
        while (reader.Read())
        {
            string product = reader.GetString(reader.GetOrdinal(PRODUCT));
            DateTime time = reader.GetDateTime(reader.GetOrdinal(TIME)).ToLocalTime();
            decimal price = reader.GetDecimal(reader.GetOrdinal(PRICE));

            data.Add(new StockData(product, time, price));
        }
    }

    public int DeleteData(string product, DateTime time)
    {
        return DataAccess.ExecuteNonQueryInternal(
            "DELETE FROM " + PRICING_DATA + " where " + PRODUCT + " = '" + product + "' AND " + TIME +
            $" > '{time:yyyy-MM-dd HH:mm:ss}';", connectionString);
    }

    public DateTime? GetMinTime(string product)
    {
        DateTime? time = null;
        try
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string cmd = $"SELECT min({TIME}) FROM {PRICING_DATA} WHERE {PRODUCT}='{product}'";
            using NpgsqlCommand command = new NpgsqlCommand(cmd, connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            time = reader.GetDateTime(0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return time;
    }
    
    public DateTime? GetMaxTime(string product)
    {
        DateTime? time = null;
        try
        {
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            string cmd = $"SELECT max({TIME}) FROM {PRICING_DATA} WHERE {PRODUCT}='{product}'";
            using NpgsqlCommand command = new NpgsqlCommand(cmd, connection);
            using NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            time = reader.GetDateTime(0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return time;
    }
}