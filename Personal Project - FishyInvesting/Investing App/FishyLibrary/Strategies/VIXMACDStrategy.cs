using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class VIXMACDStrategy: IStrategy
{
    public Parameters Parameters { get; }
    private readonly IStrategy _macdStrategy;
    private IIndicator _vixIndicator;
    
    public int NumProducts
    {
        get => 2;
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
    
    public double VixMax
    {
        get => Parameters.GetParameter(nameof(VixMax));
    }
    
    public double VixMin
    {
        get => Parameters.GetParameter(nameof(VixMin));
    }
    
    public double MaxPeriod
    {
        get => Math.Max(PeriodShort, Math.Max(PeriodLong, PeriodExponential*4));
    }
    
    public VIXMACDStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(PeriodShort), null);
        Parameters.AddParameter(nameof(PeriodLong), null);
        Parameters.AddParameter(nameof(PeriodExponential), null);
        Parameters.AddParameter(nameof(VixMax), null);
        Parameters.AddParameter(nameof(VixMin), null);
        _macdStrategy = new MacdStrategy();
        _vixIndicator = new VIXIndicator(()=>VixMax, ()=>VixMin);
    }
    

    public double GenerateSignal(List<double> price)
    {
        if (_vixIndicator.GenerateNextValue(price[1]) != 0.0)
        {
            return _macdStrategy.GenerateSignal(price);
        }
        return 0.0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        var parameters1 = new Parameters();
        parameters1.AddParameter(nameof(PeriodShort), PeriodShort);
        parameters1.AddParameter(nameof(PeriodLong), PeriodLong);
        parameters1.AddParameter(nameof(PeriodExponential), PeriodExponential);
        _macdStrategy.ResetStrategy(parameters1);
        _vixIndicator.ResetIndicator();
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "macdDictionary", _macdStrategy.IntermediateOutputFile() },
            { "vixDictionary", _vixIndicator.IntermediateOutputFile() },
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _vixIndicator.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["vixDictionary"].ToString()!)!);
        _macdStrategy.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["macdDictionary"].ToString()!)!);
    }
}