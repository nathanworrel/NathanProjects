using FishyLibrary.Strategies;

namespace CommonServices.Services;

public interface IStrategyHelperService
{
    public IStrategy GetStrategy(string strategyName);
}