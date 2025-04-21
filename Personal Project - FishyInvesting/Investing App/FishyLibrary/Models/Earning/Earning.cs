namespace FishyLibrary.Models.Earning;

public class Earning : EarningBase
{
    public int StrategyId { get; }
    public decimal TotalProfits { get; }

    public Earning(List<Return> dailyRtns, int strategyId, decimal totalProfits) : base(dailyRtns)
    {
        StrategyId = strategyId;
        TotalProfits = totalProfits;
    }
}