using FishyLibrary.Indicators;
using FishyLibrary.Models.Parameters;
using Newtonsoft.Json;

namespace FishyLibrary.Strategies;

public class PolynomialStrategy : IStrategy
{
    public Parameters Parameters { get; }
    public int NumProducts { get => 1; }
    public double MaxPeriod { get => Period; }
    
    private IIndicator _polynomialIndicator;
    
    public double Period
    {
        get => Parameters.GetParameter(nameof(Period));
    }

    public PolynomialStrategy()
    {
        Parameters = new Parameters();
        Parameters.AddParameter(nameof(Period), null);
        _polynomialIndicator = new PolynomialIndicator(() => Period);
    }

    public double GenerateSignal(List<double> price)
    {
        var prediction = _polynomialIndicator.GetValue();
        _polynomialIndicator.UpdateIndicator(price[0]);
        return prediction < price[0] ? 1 : 0;
    }

    public void ResetStrategy(Parameters parameters)
    {
        Parameters.ResetParameters(parameters);
        _polynomialIndicator.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "polynomialIndicator", _polynomialIndicator.IntermediateOutputFile() },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _polynomialIndicator.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["polynomialIndicator"].ToString()!)!);
    }
}