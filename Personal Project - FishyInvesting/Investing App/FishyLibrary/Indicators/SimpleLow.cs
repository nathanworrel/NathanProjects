using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class SimpleLow: IIndicator
{
    private Queue<double> _numbers;
    private readonly Func<double> _getLength;
    
    public SimpleLow(Func<double> getLength)
    {
        _getLength = getLength;
        _numbers = new();
    }

    public void UpdateIndicator(double newPrice)
    {
        _numbers.Enqueue(newPrice);
        if (_numbers.Count > _getLength())
        {
            _numbers.Dequeue();
        }
    }

    public double GetValue()
    {
        return _numbers.Min();
    }

    public void ResetIndicator()
    {
        _numbers = new();
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Numbers", _numbers.ToArray() }
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Numbers", out var numbersValue) && numbersValue is JArray numbers)
        {
            _numbers = new Queue<double>(numbers.ToObject<List<double>>()!);
        }
    }
}