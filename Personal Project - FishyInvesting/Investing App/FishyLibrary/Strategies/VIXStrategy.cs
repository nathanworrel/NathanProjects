using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class VIXStrategy : IStrategy
{
    public Parameters Parameters { get; }
    private IIndicator _vixIndicator;
    public int NumProducts { get => 2; }
    public double MaxPeriod { get=>2; }
    
    public double VixMax
    {
        get => Parameters.GetParameter(nameof(VixMax));
    }
    
    public double VixMin
    {
        get => Parameters.GetParameter(nameof(VixMin));
    }

    public VIXStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(VixMax), null);
        Parameters.AddParameter(nameof(VixMin), null);
        _vixIndicator = new VIXIndicator(()=>VixMax, ()=>VixMin);
    }

    public double GenerateSignal(List<double> price)
    {
        if (_vixIndicator.GenerateNextValue(price[1]) != 0.0)
        {
            return 1.0;
        }
        return 0.0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _vixIndicator.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "vixDictionary", _vixIndicator.IntermediateOutputFile() },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _vixIndicator.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["vixDictionary"].ToString()!)!);
    }
}