using System.Reflection;
using FishyLibrary.Strategies;
using Microsoft.Extensions.Logging;

namespace CommonServices.Services;

public class StrategyHelperService : IStrategyHelperService
{
    private readonly ILogger<StrategyHelperService> _logger;

    public StrategyHelperService(ILogger<StrategyHelperService> logger)
    {
        _logger = logger;
    }

    public IStrategy GetStrategy(string strategyName)
    {
        if (Activator.CreateInstance(typeof(IStrategy).GetTypeInfo().Assembly.ToString()
                , $"FishyLibrary.Strategies.{strategyName}")?.Unwrap() is IStrategy strategy)
        {
            return strategy;
        }
        throw new Exception("Strategy not found");
    }
}