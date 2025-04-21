using System.Text.Json;

namespace FishyLibrary.Indicators;

public class ExponentialMovingAverage: IIndicator
{
    private double _total;
    private readonly Func<double> _getLength;
    
    public ExponentialMovingAverage(Func<double> getLength)
    {
        _getLength = getLength;
        _total = 0;
    }
    
    public void UpdateIndicator(double newPrice) 
    {
        double k = 2 / (_getLength() + 1);
        _total = (newPrice * k) + (_total * (1 - k));
    }
    
    public double GetValue()
    {
        return _total;
    }
    
    public void ResetIndicator()
    {
        _total = 0;
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Total", _total },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Total", out var totalValue) && totalValue is double total)
        {
            _total = total;
        }
    }
}
