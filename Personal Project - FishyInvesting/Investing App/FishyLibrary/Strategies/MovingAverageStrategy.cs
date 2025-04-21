using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class MovingAverageStrategy : IStrategy
{
    public Parameters Parameters { get; }
    private readonly IIndicator _simpleMovingAverage;
    private readonly IIndicator _simpleMovingAverage2;

    public int NumProducts
    {
        get => 1;
    }

    public double Period
    {
        get => Parameters.GetParameter(nameof(Period));
    }

    public double Period2
    {
        get => Parameters.GetParameter(nameof(Period2));
    }

    public double MaxPeriod
    {
        get => Math.Max(Period, Period2);
    }

    public MovingAverageStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(Period), null);
        Parameters.AddParameter(nameof(Period2), null);
        _simpleMovingAverage = new SimpleMovingAverage((() => Period));
        _simpleMovingAverage2 = new SimpleMovingAverage((() => Period2));
    }

    public double GenerateSignal(List<double> price)
    {
        return _simpleMovingAverage2.GenerateNextValue(price[0]) < _simpleMovingAverage.GenerateNextValue(price[0])
            ? 1
            : 0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _simpleMovingAverage.ResetIndicator();
        _simpleMovingAverage2.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "simpleMovingAverage", _simpleMovingAverage.IntermediateOutputFile() },
            { "simpleMovingAverage2", _simpleMovingAverage2.IntermediateOutputFile() },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _simpleMovingAverage.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string, Object>>(
                intermediateData["simpleMovingAverage"].ToString()!)!);
        _simpleMovingAverage2.SetIntermediateValues(
            JsonConvert.DeserializeObject<Dictionary<string, Object>>(intermediateData["simpleMovingAverage2"]
                .ToString()!)!);
    }
}