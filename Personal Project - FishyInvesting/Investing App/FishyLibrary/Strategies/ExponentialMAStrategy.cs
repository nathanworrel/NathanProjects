using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class ExponentialMAStrategy: IStrategy
{
    public Parameters Parameters { get; }
    private readonly IIndicator _exponentialMovingAverage;
    private double _previous;
    public int NumProducts
    {
        get => 1;
    }

    public double Period
    {
        get => Parameters.GetParameter(nameof(Period));
    }
    
    public double MaxPeriod
    {
        get => Period;
    }
    public ExponentialMAStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(Period), null);

        _exponentialMovingAverage = new ExponentialMovingAverage((() => Period));
        _previous = 0;
    }
    
    public double GenerateSignal(List<double> price)
    {
        var nextValue = _exponentialMovingAverage.GenerateNextValue(price[0]);
        double signal = nextValue > _previous ? 1 : 0;
        _previous = nextValue;
        return signal;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _previous = 0;
        _exponentialMovingAverage.ResetIndicator();
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            {"exponentialMovingAverage", _exponentialMovingAverage.IntermediateOutputFile()},
            {"previous", _previous}
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _exponentialMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["exponentialMovingAverage"].ToString()!)!);
        
        if (intermediateData.TryGetValue("previous", out var previousValue) && previousValue is double previous)
        {
            _previous = previous;
        }
    }
}