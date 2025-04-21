
namespace FishyLibrary.Models.StrategyRuntime;

public class StrategyRuntime : StrategyRuntimeBase
{
    public int StrategyId { get; set; }
    public Strategy.Strategy Strategy { get; set; }
    
    public StrategyRuntime() {}

    public StrategyRuntime(int id, int strategyId, TimeSpan strategyRuntime)
    {
        Id = id;
        StrategyId = strategyId;
        Runtime = strategyRuntime;
    }
}