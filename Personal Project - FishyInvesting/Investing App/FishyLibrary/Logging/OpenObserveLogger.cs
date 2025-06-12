using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FishyLibrary.Logging;

public class OpenObserveLogger : ILogger
{
    private readonly string _name;

    public OpenObserveLogger(string name)
    {
        _name = name;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var logEntry = new LogEntry
        {
            LogLevel = logLevel.ToString(),
            LogSource = _name,
            Message = formatter(state, exception),
            CreatedTime = DateTime.UtcNow
        };

        // Save logEntry to the database
        SaveLog(logEntry);
    }

    private void SaveLog(LogEntry logEntry)
    {
        // Implement database save logic here
        // Define the API endpoint and credentials
        var url = "";
        var username = "";
        var password = "";

        // Prepare the data to be sent
        var jsonData = JsonConvert.SerializeObject(logEntry);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var task = SendToOpenObserve(username, password, url, content).Result;
    }

    private async Task<bool> SendToOpenObserve(string username, string password, string url, StringContent content)
    {
        try
        {
            // Use HttpClient to send the request
            using (var client = new HttpClient(new HttpClientHandler
                   {
                       ServerCertificateCustomValidationCallback =
                           HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                   }))
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                // Add Basic Authentication header
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Send POST request
                var response = await client.PostAsync(url, content);

                // Read and print the response
                string responseString = await response.Content.ReadAsStringAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred logging '{content}' to OpenObserve: {ex.Message}");
            return false;
        }
    }
}