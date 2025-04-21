namespace WebApi.StrategyService.Services;

public interface IStrategyServiceService
{
    void StrategyRun(int strategyId, TimeSpan runTime);
}