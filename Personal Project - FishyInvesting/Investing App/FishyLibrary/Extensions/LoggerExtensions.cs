using FishyLibrary.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FishyLibrary.Extensions;

public static class LoggerExtensions
{
    public static ILoggingBuilder AddOpenObserveLogger(this ILoggingBuilder builder)  
    {
        builder.Services.AddSingleton<ILoggerProvider, OpenObserveLoggerProvider>();  
        return builder;  
    } 
    
    public static ILoggingBuilder RouteLogger(this ILoggingBuilder builder)  
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            builder.ClearProviders();
            builder.AddSeq(); // NOTE: sends logs to port 1 which is default port
        }
        return builder;  
    } 
}