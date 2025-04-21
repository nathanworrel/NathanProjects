using FishyLibrary.Models;
using FishyLibrary.Models.Earning;
using FishyLibrary.Models.Parameters;

namespace FishyBacktesty.Models;

public class EarningBacktest : EarningBase
{
    public Parameters Parameters { get; set; }
    public String MostRecentSplit { get; set; }

    public EarningBacktest(List<Return> dailyRtns, Parameters parameters) : base(dailyRtns)
    {
        Parameters = parameters;
        MostRecentSplit = "";
    }
}