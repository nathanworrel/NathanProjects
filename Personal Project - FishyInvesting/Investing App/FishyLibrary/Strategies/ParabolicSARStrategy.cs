using FishyLibrary.Helpers;
using FishyLibrary.Indicators;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class ParabolicSARStrategy : IStrategy
{
    public Parameters Parameters { get; }
    private IIndicator _parabolicSAR;
    private IIndicator _movingAverage;

    public int NumProducts
    {
        get => 1;
    }

    public double StepAF
    {
        get => Parameters.GetParameter(nameof(StepAF));
    }

    public double PeriodHistory
    {
        get => Parameters.GetParameter(nameof(PeriodHistory));
    }

    public double PeriodMA
    {
        get => Parameters.GetParameter(nameof(PeriodMA));
    }

    public double MaxPeriod
    {
        get => Math.Max(PeriodHistory, PeriodMA) * 2;
    }

    public ParabolicSARStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(StepAF), 0.02);
        Parameters.AddParameter(nameof(PeriodHistory), 5);
        Parameters.AddParameter(nameof(PeriodMA), 5);

        _parabolicSAR = new ParabolicSAR(() => StepAF, () => PeriodHistory);
        _movingAverage = new SimpleMovingAverage(() => MaxPeriod);
    }

    public double GenerateSignal(List<double> price)
    {
        double sar = _parabolicSAR.GenerateNextValue(price[0]);
        double movingAverage = _movingAverage.GenerateNextValue(price[0]);
        return sar > price[0] && price[0] > movingAverage ? 0 : 1;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _parabolicSAR.ResetIndicator();
        _movingAverage.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "simpleMovingAverageDictionary", _movingAverage.IntermediateOutputFile() },
            { "parabolicSarDictionary", _parabolicSAR.IntermediateOutputFile() }
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _parabolicSAR.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["parabolicSarDictionary"].ToString()!)!);
        _movingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["simpleMovingAverageDictionary"].ToString()!)!);
    }
}