namespace FishyLibrary.Logging;

public class LogEntry
{
    public string LogLevel { get; set; }
    public string LogSource { get; set; }
    public string Message { get; set; }
    public DateTime CreatedTime { get; set; }
}