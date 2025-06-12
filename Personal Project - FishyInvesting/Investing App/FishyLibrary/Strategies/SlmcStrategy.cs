using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

// Signal Line Momentum Crossover Strategy
public class SlmcStrategy : IStrategy
{
    public Parameters Parameters { get; }
    private readonly IIndicator _rateOfChange;
    private readonly IIndicator _momentumEma;
    private readonly IIndicator _pomLineEma;
    private readonly IIndicator _pmoSignalLineEma;

    public SlmcStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(MomentumPeriod), 35);
        Parameters.AddParameter(nameof(PomPeriod), 20);
        Parameters.AddParameter(nameof(SignalPeriod), 10);
        Parameters.AddParameter(nameof(MomentumMultiplier), 10);
        
        _rateOfChange = new RateOfChange();
        _momentumEma = new ExponentialMovingAverage(() => MomentumPeriod);
        _pomLineEma = new ExponentialMovingAverage(() => PomPeriod);
        _pmoSignalLineEma = new ExponentialMovingAverage(() => SignalPeriod);
    }

    public int NumProducts
    {
        get => 1;
    }

    public double MaxPeriod
    {
        get => 1;
    }

    public double MomentumPeriod
    {
        get => Parameters.GetParameter(nameof(MomentumPeriod));
    }

    public double PomPeriod
    {
        get => Parameters.GetParameter(nameof(PomPeriod));
    }

    public double SignalPeriod
    {
        get => Parameters.GetParameter(nameof(SignalPeriod));
    }

    public double MomentumMultiplier
    {
        get => Parameters.GetParameter(nameof(MomentumMultiplier));
    }

    public double GenerateSignal(List<double> price)
    {
        var roc = _rateOfChange.GenerateNextValue(price.First());
        var momentum = MomentumMultiplier * _momentumEma.GenerateNextValue(roc);
        var pom = _pomLineEma.GenerateNextValue(momentum);
        var signal = _pmoSignalLineEma.GenerateNextValue(pom);
        return pom > signal ? 1 : 0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _rateOfChange.ResetIndicator();
        _momentumEma.ResetIndicator();
        _pomLineEma.ResetIndicator();
        _pmoSignalLineEma.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new Dictionary<string, object>()
        {
            {"RateOfChangeDictionary", _rateOfChange.IntermediateOutputFile() },
            {"MomentumEmaDictionary", _momentumEma.IntermediateOutputFile() },
            {"PomLineDictionary", _pomLineEma.IntermediateOutputFile() },
            {"SignalLineDictionary", _pmoSignalLineEma.IntermediateOutputFile() }
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _rateOfChange.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["RateOfChangeDictionary"].ToString()!)!);
        _momentumEma.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["MomentumEmaDictionary"].ToString()!)!);
        _pomLineEma.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["PomLineDictionary"].ToString()!)!);
        _pmoSignalLineEma.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["SignalLineDictionary"].ToString()!)!);

    }

}

// Notes:
// All the best total results
// - momentum period 33 and pom period 18 and signal period 4
// - (so momentum multiplier did not have significant effect)
// - market had higher high and lower low
// All the best avg yearly
// - momentum period 27, pom period 18, and signal period 2
// - market had the best yearly returns
// - market had higher high and lower low
// times
// - 10:40 had slightly higher returns at 9._
// - 11:40 had higher returns at 10.17
// - 12:10 had 7.95 returns and the most recent highest is not the one that was highest for longer
// - 12:30 had 11.19 returns and was highest at last 2 splits
// - BEST 12:40 had higher returns at 12.2 and it was the highest split for 4 splits in a row
// - 12:50 had 10.19 returns and the highest strategy was for the last 2/3 of the splits
// - 1:40 had the lowest returns at 7.99 and it was quite scattered the best strategy
// - 2:40 is the same as 1:40
// - 3:40 returns were no where close to beating the market

