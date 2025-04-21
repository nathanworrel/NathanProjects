namespace FishyLibrary.Models.Earning;

public class EarningBase
{
    public List<Return> DailyRtns { get; }
    
    public List<Return> MthRtns { get; }
    
    public decimal BestMthRtn { get; }

    public decimal WorstMthRtn { get; }
    
    public decimal AvgMthRtns { get; }
    
    public List<Return> YrRtns { get; }
    
    public decimal BestYrRtn { get; }
    
    public decimal WorstYrRtn { get; }

    public decimal AvgYrRtns { get; }
    
    public decimal EstYrRtns { get; }
    
    public List<Return> WorstMths { get; }
    
    public List<Return> BestMths { get; }
    
    public decimal TotalRtns { get; }
    
    public EarningBase(List<Return> dailyRtns)
    {
        DailyRtns = dailyRtns;
        MthRtns = dailyRtns.GroupBy(g => new {year=g.Date.Year , month=g.Date.Month})
            .Select(g => new Return(new DateTime(g.Key.year, g.Key.month, 1), g.Select(r => r.Returns)
                .Aggregate(1.0m, (a, b) => a * b ))).ToList();
        YrRtns = dailyRtns.GroupBy(g => g.Date.Year)
            .Select(g => new Return(new DateTime(g.Key, 1, 1), g.Select(r => r.Returns)
                .Aggregate(1.0m, (a, b) => a * b ))).ToList();
        BestMthRtn = MthRtns.Select(r => r.Returns).Max();
        WorstMthRtn = MthRtns.Select(r => r.Returns).Min();
        AvgMthRtns = MthRtns.Select(r => r.Returns).Average();
        BestYrRtn = YrRtns.Select(r => r.Returns).Max();
        WorstYrRtn = YrRtns.Select(r => r.Returns).Min();
        AvgYrRtns = YrRtns.Select(r => r.Returns).Average();
        WorstMths = MthRtns.Where(r => Math.Abs(r.Returns - WorstMthRtn) < 0.0001m).ToList();
        BestMths = MthRtns.Where(r => Math.Abs(r.Returns - BestMthRtn) < 0.0001m).ToList();
        TotalRtns = dailyRtns.Select(r => r.Returns).Aggregate(1.0m, (a, b) => a * b);
        EstYrRtns = (decimal) Math.Pow((double)TotalRtns, 1.0/YrRtns.Count());
    }
}