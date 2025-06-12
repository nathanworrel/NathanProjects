using System.Text.Json;

namespace FishyLibrary.Indicators;

public class VIXIndicator: IIndicator
{
    private double _previous;
    private readonly Func<double> _getMin;
    private readonly Func<double> _getMax;
    
    public VIXIndicator(Func<double> getMax, Func<double> getMin)
    {
        _previous = 0.0;
        _getMin = getMin;
        _getMax = getMax;
    }

    public void UpdateIndicator(double newPrice)
    {
        if (newPrice > _getMax())
        {
            _previous = 0.0;
        }
        else if (newPrice < _getMin())
        {
            _previous = 1.0;
        }
    }

    public double GetValue()
    {
        return _previous;
    }

    public void ResetIndicator()
    {
        _previous = 0.0;
    }
    
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Previous", _previous },
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Previous", out var previousValue) && previousValue is double previous)
        {
            _previous = previous;
        }
    }
}