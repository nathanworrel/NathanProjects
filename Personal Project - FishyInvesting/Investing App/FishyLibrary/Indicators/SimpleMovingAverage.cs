using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class SimpleMovingAverage: IIndicator
{

    private Queue<double> _numbers;
    private double _total;
    private readonly Func<double> _getLength;

    public SimpleMovingAverage(Func<double> getLength)
    {
        _getLength = getLength;
        _total = 0;
        _numbers = new();
    }

    public void UpdateIndicator(double newPrice)
    {
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
        return _total / _numbers.Count;
    }

    public void ResetIndicator()
    {
        _total = 0;
        _numbers = new();
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Total", _total },
            { "Numbers", _numbers.ToArray() }
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
    }

}