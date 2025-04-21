using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class LinearWeightedMovingAverage: IIndicator
{

    private Queue<double> _numbers;
    private double _total;
    private readonly Func<double> _getLength;
    private double _totalWeights;
    private double _currentTotal;

    public LinearWeightedMovingAverage(Func<double> getLength)
    {
        _getLength = getLength;
        _total = 0;
        _numbers = new();
        _totalWeights = 0;
        _currentTotal = 0;
    }

    public void UpdateIndicator(double newPrice)
    {
        _numbers.Enqueue(newPrice);
        _total += newPrice*_numbers.Count;
        _currentTotal += newPrice;
        _totalWeights += _numbers.Count;
        if (_numbers.Count > _getLength())
        {
            _total -= _currentTotal;
            _totalWeights -= _numbers.Count;
            double remove = _numbers.Dequeue();
            _currentTotal -= remove;
        }
    }

    public double GetValue() 
    {
        return _total / _totalWeights;
    }

    public void ResetIndicator()
    {
        _total = 0;
        _numbers = new();
        _totalWeights = 0;
        _currentTotal = 0;
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "Total", _total },
            { "Numbers", _numbers.ToArray() },
            { "TotalWeights", _totalWeights },
            { "CurrentTotal", _currentTotal }
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
        
        if (intermediateData.TryGetValue("TotalWeights", out var totalWeightsValue) && totalWeightsValue is double totalWeights)
        {
            _totalWeights = totalWeights;
        }
        
        if (intermediateData.TryGetValue("CurrentTotal", out var currentTotalValue) && currentTotalValue is double currentTotal)
        {
            _currentTotal = currentTotal;
        }
    }

}