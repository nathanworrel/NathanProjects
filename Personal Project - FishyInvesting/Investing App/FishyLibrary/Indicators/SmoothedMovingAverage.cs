using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class SmoothedMovingAverage: IIndicator
{
    private readonly Func<double> _getLength;
    private Queue<double> _numbers;
    private double _total;
    private double _current;

    public SmoothedMovingAverage(Func<double> getLength)
    {
        _getLength = getLength;
        _numbers = new();
        _total = 0;
        _current = 0;
    }

    public void UpdateIndicator(double newPrice)
    {
        _current = (_total - _current + newPrice)/(_numbers.Count+1);
        
        _numbers.Enqueue(newPrice);
        _total += newPrice;
        if (_numbers.Count > _getLength())
        {
            double remove = _numbers.Dequeue();
            _total -= remove;
        }
    }

    public double GetValue() 
    {
        return _current;
    }

    public void ResetIndicator()
    {
        _numbers = new();
        _total = 0;
        _current = 0;
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Total", _total },
            { "Numbers", _numbers.ToArray() },
            { "Current", _current }
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Numbers", out var numbersValue) && numbersValue is JArray numbers)
        {
            _numbers = new Queue<double>(numbers.ToObject<List<double>>()!);
        }
        
        if (intermediateData.TryGetValue("Total", out var totalValue) && totalValue is double total)
        {
            _total = total;
        }
        
        if (intermediateData.TryGetValue("Current", out var currentValue) && currentValue is double current)
        {
            _current = current;
        }
    }

}