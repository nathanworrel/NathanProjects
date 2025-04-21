using System.Text.Json;
using Newtonsoft.Json;

namespace FishyLibrary.Indicators;

public class MACD: IIndicator
{
    private IIndicator _shortMovingAverage;
    private IIndicator _longMovingAverage;
    private IIndicator _exponentialMovingAverage;

    public MACD(Func<double> getLengthShort, Func<double> getLengthLong, Func<double> getLengthExponential)
    {
        _shortMovingAverage = new SimpleMovingAverage(getLengthShort);
        _longMovingAverage = new SimpleMovingAverage(getLengthLong);
        _exponentialMovingAverage = new ExponentialMovingAverage(getLengthExponential);
    }

    public void UpdateIndicator(double newPrice)
    {
        double shortMA = _shortMovingAverage.GenerateNextValue(newPrice);
        double longMA = _longMovingAverage.GenerateNextValue(newPrice);
        _exponentialMovingAverage.UpdateIndicator(longMA - shortMA);
    }

    public double GetValue()
    {
        double difference = _longMovingAverage.GetValue() - _shortMovingAverage.GetValue();
        return difference - _exponentialMovingAverage.GetValue(); 
    }

    public void ResetIndicator()
    {
        _shortMovingAverage.ResetIndicator();
        _longMovingAverage.ResetIndicator();
        _exponentialMovingAverage.ResetIndicator();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "shortData", _shortMovingAverage.IntermediateOutputFile() },
            { "longData", _longMovingAverage.IntermediateOutputFile() },
            { "exponentialData", _exponentialMovingAverage.IntermediateOutputFile() },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        _shortMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["shortData"].ToString()!)!);
        _longMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["longData"].ToString()!)!);
        _exponentialMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["exponentialData"].ToString()!)!);
    }
}
