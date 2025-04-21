using FishyBacktesty.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;

namespace UnitTests;

public class BacktestEfficiencyTesting
{
    [Fact]
    public void OneMonth()
    {
        List<Return> backtestReturns = new List<Return>()
        {
            new Return(new DateTime(2024, 2, 1), 1),
            new Return(new DateTime(2024, 2, 2), 1.2m),
            new Return(new DateTime(2024, 2, 3), 1.1m),
            new Return(new DateTime(2024, 2, 4), 0.7m),
        };
        EarningBacktest efficiency = new EarningBacktest(backtestReturns, new Parameters());
        Assert.Single(efficiency.MthRtns);
        Assert.Single(efficiency.YrRtns);
        Assert.Equal(new DateTime(2024, 2,1), efficiency.MthRtns.First().Date);
        Assert.Equal(0.924, (double)efficiency.MthRtns.First().Returns, 3);
        Assert.Equal(new DateTime(2024, 1,1), efficiency.YrRtns.First().Date);
        Assert.Equal(0.924, (double)efficiency.YrRtns.First().Returns, 3);
        Assert.Equal(0.924, (double)efficiency.TotalRtns, 3);
    }
    
    [Fact]
    public void MultipleMonth()
    {
        List<Return> backtestReturns = new List<Return>()
        {
            new Return(new DateTime(2024, 2, 1), 0.9m),
            new Return(new DateTime(2024, 2, 2), 1.2m),
            new Return(new DateTime(2024, 3, 3), 1.1m),
            new Return(new DateTime(2024, 3, 4), 1.4m),
            new Return(new DateTime(2024, 4, 5), 0.9m),
            new Return(new DateTime(2024, 4, 6), 0.7m),
        };
        EarningBacktest efficiency = new EarningBacktest(backtestReturns, new Parameters());
        Assert.Equal(3, efficiency.MthRtns.Count);
        Assert.Single(efficiency.YrRtns);
        
        Assert.Equal(new DateTime(2024, 2,1), efficiency.MthRtns.First().Date);
        Assert.Equal(1.08, (double)efficiency.MthRtns.First().Returns, 2);
        Assert.Equal(new DateTime(2024, 3,1), efficiency.MthRtns[1].Date);
        Assert.Equal(1.54, (double)efficiency.MthRtns[1].Returns, 2);
        Assert.Equal(new DateTime(2024, 4,1), efficiency.MthRtns.Last().Date);
        Assert.Equal(0.63, (double)efficiency.MthRtns.Last().Returns, 2);
        
        Assert.Equal(new DateTime(2024, 1,1), efficiency.YrRtns.First().Date);
        Assert.Equal(1.05, (double)efficiency.YrRtns.First().Returns, 2);
        Assert.Equal(1.05, (double)efficiency.TotalRtns, 2);
    }
    
    [Fact]
    public void MultipleYears()
    {
        List<Return> backtestReturns = new List<Return>()
        {
            new Return(new DateTime(2024, 2, 1), 0.9m),
            new Return(new DateTime(2024, 2, 2), 1.2m),
            new Return(new DateTime(2024, 2, 3), 1.1m),
            new Return(new DateTime(2025, 4, 4), 1.4m),
            new Return(new DateTime(2025, 4, 5), 0.9m),
            new Return(new DateTime(2025, 4, 6), 0.7m),
        };
        EarningBacktest efficiency = new EarningBacktest(backtestReturns, new Parameters());
        Assert.Equal(2, efficiency.MthRtns.Count);
        Assert.Equal(2, efficiency.YrRtns.Count);
        
        Assert.Equal(new DateTime(2024, 2,1), efficiency.MthRtns.First().Date);
        Assert.Equal(1.19, (double)efficiency.MthRtns.First().Returns, 2);
        Assert.Equal(new DateTime(2025, 4,1), efficiency.MthRtns.Last().Date);
        Assert.Equal(0.88, (double)efficiency.MthRtns.Last().Returns, 2);
        
        Assert.Equal(new DateTime(2024, 1,1), efficiency.YrRtns.First().Date);
        Assert.Equal(1.19, (double)efficiency.YrRtns.First().Returns, 2);
        Assert.Equal(new DateTime(2025, 1,1), efficiency.YrRtns.Last().Date);
        Assert.Equal(0.88, (double)efficiency.YrRtns.Last().Returns, 2);
        Assert.Equal(1.05, (double)efficiency.TotalRtns, 2);
    }
    
    [Fact]
    public void MultipleYears_MultipleMonths()
    {
        List<Return> backtestReturns = new List<Return>()
        {
            new Return(new DateTime(2024, 2, 1), 0.9m),
            new Return(new DateTime(2024, 2, 2), 1.2m),
            new Return(new DateTime(2024, 3, 3), 1.1m),
            new Return(new DateTime(2025, 4, 4), 1.4m),
            new Return(new DateTime(2025, 4, 5), 0.9m),
            new Return(new DateTime(2025, 5, 6), 0.7m),
        };
        EarningBacktest efficiency = new EarningBacktest(backtestReturns, new Parameters());
        Assert.Equal(4, efficiency.MthRtns.Count);
        Assert.Equal(2, efficiency.YrRtns.Count);
        Assert.Equal(1.05, (double)efficiency.TotalRtns, 2);
    }
}