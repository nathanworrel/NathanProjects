using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;

namespace WebApi.DB.Access.Service;

public interface IStrategyRuntimeService
{
    StrategyRuntimeGet? Get(int id);
    StrategyRuntime? Find(int id);
    List<StrategyRuntimeGet> GetAll();
    List<StrategyRuntimeGet> GetAllByStrategy(int strategyId);
    int Add(int strategyId, string time = "9:30:00");
    StrategyRuntimePost Delete(StrategyRuntime strategyRuntime);
    void SetRuntimes(Strategy strategy, List<StrategyRuntimeString> times);
}