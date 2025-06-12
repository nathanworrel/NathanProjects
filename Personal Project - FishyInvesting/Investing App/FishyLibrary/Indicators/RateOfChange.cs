namespace FishyLibrary.Indicators;

public class RateOfChange : IIndicator
{
    private double _yesterdaysPrice;
    private double _todayPrice;
    
    public RateOfChange()
    {
        _yesterdaysPrice = 1;
    }

    public void UpdateIndicator(double value)
    {
        _yesterdaysPrice = _todayPrice;
        _todayPrice = value;
    }

    public double GetValue()
    {
        return (_todayPrice - _yesterdaysPrice) / _yesterdaysPrice * 100;
    }

    public void ResetIndicator()
    {
        _yesterdaysPrice = 1;
        _todayPrice = 1;
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new Dictionary<string, object>()
        {
            { "YesterdayPrice", _yesterdaysPrice },
            { "TodayPrice", _todayPrice },
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("YesterdayPrice", out var yesterdaysPrice) && yesterdaysPrice is double price)
        {
            _yesterdaysPrice = price;
        }
        if (intermediateData.TryGetValue("TodayPrice", out var todayPrice) && todayPrice is double todaysPrice)
        {
            _yesterdaysPrice = todaysPrice;
        }
    }
}