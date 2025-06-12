using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class StandardDeviation: IIndicator
{
    private readonly Func<double> _getLength;
    private IIndicator _simpleMovingAverage;
    private Queue<double> _numbers;
    
    public StandardDeviation(Func<double> getLength)
    {
        _getLength = getLength;
        _numbers = new();
        _simpleMovingAverage = new SimpleMovingAverage(getLength);
    }
    
    public void UpdateIndicator(double newPrice) 
    {
        _simpleMovingAverage.UpdateIndicator(newPrice);
        _numbers.Enqueue(newPrice);
        if (_numbers.Count > _getLength())
        {
            _numbers.Dequeue();
        }
    }
    
    public double GetValue()
    {
        var average = _simpleMovingAverage.GetValue();
        double sum = 0;
        foreach (var number in _numbers)
        {
            sum += Math.Pow(number-average,2);
        }
        
        sum /= _numbers.Count-1;

        return Math.Sqrt(sum);
    }
    
    public void ResetIndicator()
    {
        _simpleMovingAverage.ResetIndicator();
        _numbers = new();
    }
    
    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "maData", _simpleMovingAverage.IntermediateOutputFile() },
            { "Numbers", _numbers.ToArray() }
        };
    }
    
    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("Numbers", out var numbersValue) && numbersValue is JArray numbers)
        {
            _numbers = new Queue<double>(numbers.ToObject<List<double>>()!);
        }

        _simpleMovingAverage.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(intermediateData["maData"].ToString()!)!);
    }
}