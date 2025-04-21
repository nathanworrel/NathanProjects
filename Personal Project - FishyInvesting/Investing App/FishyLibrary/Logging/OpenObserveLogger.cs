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

        // Prepare the data to be sent
        var jsonData = JsonConvert.SerializeObject(logEntry);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        
        //changed logic********
        Console.Out.WriteLine(content);
    }
    
}