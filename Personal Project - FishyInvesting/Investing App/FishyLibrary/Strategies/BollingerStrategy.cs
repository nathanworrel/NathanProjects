using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class BollingerStrategy : IStrategy
{
    public Parameters Parameters { get; }
    private IIndicator _simpleMovingAverage;
    private IIndicator _standardDeviation;
    double previousValue;

    public int NumProducts
    {
        get => 1;
    }

    public double Period
    {
        get => Parameters.GetParameter(nameof(Period));
    }

    public double NumDeviation
    {
        get => Parameters.GetParameter(nameof(NumDeviation));
    }

    public double MaxPeriod
    {
        get => Parameters.GetParameter(nameof(Period));
    }

    public BollingerStrategy()
    {
        previousValue = 0.0;
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(Period), null);
        Parameters.AddParameter(nameof(NumDeviation), null);
        _simpleMovingAverage = new SimpleMovingAverage(() => Period);
        _standardDeviation = new StandardDeviation(() => Period);
    }
    
    public double GenerateSignal(List<double> price)
    {
        double average = _simpleMovingAverage.GenerateNextValue(price[0]);
        double deviation = _standardDeviation.GenerateNextValue(price[0]);
        double low = average - (deviation * NumDeviation);
        double high = average + (deviation * NumDeviation);
        if (price[0] > high)
        {
            previousValue = 0.0;
        }
        else if (price[0] < low)
        {
            previousValue = 1.0;
        }

        return previousValue;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _simpleMovingAverage.ResetIndicator();
        _standardDeviation.ResetIndicator();
        previousValue = 0.0;
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "simpleMovingAverageDictionary", _simpleMovingAverage.IntermediateOutputFile() },
            { "standardDeviationDictionary", _standardDeviation.IntermediateOutputFile() },
            { "previousValue", previousValue },
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        
        if (intermediateData.TryGetValue("previousValue", out var previousValueString) &&
            previousValueString is double previousValueDouble)
        {
            previousValue = previousValueDouble;
        }

        _simpleMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["simpleMovingAverageDictionary"].ToString()!)!);
        _standardDeviation.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["standardDeviationDictionary"].ToString()!)!);
    }
}