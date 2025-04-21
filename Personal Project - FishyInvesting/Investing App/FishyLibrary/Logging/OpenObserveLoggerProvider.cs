using Microsoft.Extensions.Logging;

namespace FishyLibrary.Logging;

public class OpenObserveLoggerProvider : ILoggerProvider
{
    public OpenObserveLoggerProvider() {}

    public ILogger CreateLogger(string categoryName)
    {
        return new OpenObserveLogger(categoryName);
    }

    public void Dispose() { }
}