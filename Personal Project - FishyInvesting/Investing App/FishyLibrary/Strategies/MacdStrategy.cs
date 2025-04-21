using FishyLibrary.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class MacdStrategy: IStrategy
{
    public Parameters Parameters { get; }
    private readonly IIndicator _macD;
    
    public int NumProducts
    {
        get => 1;
    }

    public double PeriodShort
    {
        get => Parameters.GetParameter(nameof(PeriodShort));
    }

    public double PeriodLong
    {
        get => Parameters.GetParameter(nameof(PeriodLong));
    }

    public double PeriodExponential
    {
        get => Parameters.GetParameter(nameof(PeriodExponential));
    }

    public double MaxPeriod
    {
        get => Math.Max(PeriodExponential*4, Math.Max(PeriodShort, PeriodLong));
    }

    public MacdStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(PeriodShort), null);
        Parameters.AddParameter(nameof(PeriodLong), null);
        Parameters.AddParameter(nameof(PeriodExponential), null);
        
        _macD = new MACD((() => PeriodShort), (() => PeriodLong), (() => PeriodExponential));
    }
    
    public double GenerateSignal(List<double> price)
    {
        return 0 < _macD.GenerateNextValue(price[0]) ? 1 : 0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _macD.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "macdDictionary", _macD.IntermediateOutputFile() },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _macD.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["macdDictionary"].ToString()!)!);
    }
}
