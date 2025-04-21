using FishyLibrary.Models;

namespace FishyLibrary.Utils;

public class ApiBaseUrls
{
    private static readonly string Localhost = "localhost";
    private static readonly string DockerPort = "1";

    private static readonly Dictionary<Api, string> ApiToPort = new Dictionary<Api, string>()
    {
        { Api.GetData, "1" },
        { Api.MakeTrade, "1" },
        { Api.CharlesSchwab, "1" },
        { Api.Scheduler, "1" },
        { Api.StrategyService, "1" },
        { Api.UpdateTrades, "1" },
        { Api.DbAccess, "1" },
        { Api.Logs, "1"}
    };
    
    private static readonly Dictionary<Api, string> ApiToPortDevelopment = new Dictionary<Api, string>()
    {
        { Api.GetData, "1" },
        { Api.MakeTrade, "1" },
        { Api.CharlesSchwab, "1" },
        { Api.Scheduler, "1" },
        { Api.StrategyService, "1" },
        { Api.UpdateTrades, "1" },
        { Api.DbAccess, "1" },
        { Api.Logs, "1"}
    };

    private static readonly Dictionary<Api, string> ApiToHost = new Dictionary<Api, string>()
    {
        { Api.GetData, "get-data" },
        { Api.MakeTrade, "make-trade" },
        { Api.CharlesSchwab, "charles-schwab" },
        { Api.Scheduler, "scheduler" },
        { Api.StrategyService, "strategy-service" },
        { Api.UpdateTrades, "update-trades" },
        { Api.DbAccess, "db-access" },
        { Api.Logs, "logs"}
    };

    public static string BaseUrl(string environment, Api api)
    {
        if (environment == "Development")
        {
            return $"http://{Localhost}:{ApiToPortDevelopment[api]}/";
        }

        if (environment == "Staging")
        {
            return $"http://{Localhost}:{ApiToPort[api]}/";
        }

        if (environment == "Production")
        {
            return $"http://{ApiToHost[api]}:{DockerPort}/";
        }

        return $"http://{ApiToHost[api]}:{DockerPort}/";
    }
}