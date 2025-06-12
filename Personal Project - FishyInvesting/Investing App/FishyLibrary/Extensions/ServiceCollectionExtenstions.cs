using EasyNetQ;
using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.Span;
using SerilogTracing;

namespace FishyLibrary.Extensions;

public static class ServiceCollectionExtenstions
{
    public static IServiceCollection AddSerilogWithSeq(this IServiceCollection services)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            services.AddSerilog(lc => lc 
                .Enrich.With<ActivityEnricher>()
                .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://logs:1"));
        }
        else if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Staging")
        {
            
            services.AddSerilog(lc => lc 
                .Enrich.With<ActivityEnricher>()
                .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:1"));
        }
        else
        {
            services.AddSerilog(lc => lc.WriteTo.Console());
        }
        return services;
    }

    public static IServiceCollection CustomAddEasyNetQ(this IServiceCollection services)
    {
        string host = "localhost";
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            host = "rabbitmq";
        }
        services.AddEasyNetQ($"host={host}");
        
        return services;
    }
}