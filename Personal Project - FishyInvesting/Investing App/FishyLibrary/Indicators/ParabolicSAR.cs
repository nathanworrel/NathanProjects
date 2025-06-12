using Newtonsoft.Json.Linq;

namespace FishyLibrary.Indicators;

public class ParabolicSAR : IIndicator
{
    private double _currentSAR;
    private double _extremePrice;
    private double _accelerationFactor;
    private double _maxAF;
    private readonly Func<double> _getStepAF; //Recommend 0.02    
    private Queue<double> _priceHistory;
    private readonly Func<double> _priceHistorySize;
    private bool _isUptrend;

    public ParabolicSAR(Func<double> getStepAF, Func<double> priceHistorySize)
    {
        _priceHistorySize = priceHistorySize;
        _getStepAF = getStepAF;
        _currentSAR = 0.0;
        _accelerationFactor = _getStepAF();
        _maxAF = _getStepAF() * 10;
        _isUptrend = true;
        _extremePrice = 0.0;
        _priceHistory = new Queue<double>();
    }

    private void UpdatePriceHistory(double newPrice)
    {
        if (_priceHistory.Count == _priceHistorySize())
        {
            _priceHistory.Dequeue();
        }

        _priceHistory.Enqueue(newPrice);
    }

    private double GetPreviousHigh()
    {
        return _priceHistory.Max();
    }

    private double GetPreviousLow()
    {
        return _priceHistory.Min();
    }

    public void UpdateIndicator(double newPrice)
    {
        UpdatePriceHistory(newPrice);

        if (_currentSAR == 0.0 && _extremePrice == 0.0)
        {
            _currentSAR = newPrice;
            _extremePrice = newPrice;
            return;
        }

        double nextSAR = _currentSAR + _accelerationFactor * (_extremePrice - _currentSAR);

        if (_isUptrend)
        {
            if (newPrice < _currentSAR)
            {
                // Reversal to downtrend
                _isUptrend = false;
                _currentSAR = _extremePrice;
                _extremePrice = newPrice;
                _accelerationFactor = _getStepAF();
            }
            else
            {
                // Continue uptrend
                if (newPrice > _extremePrice)
                {
                    _extremePrice = newPrice;
                    _accelerationFactor = Math.Min(_accelerationFactor + _getStepAF(), _maxAF);
                }

                _currentSAR = Math.Min(nextSAR, GetPreviousLow());
            }
        }
        else
        {
            if (newPrice > _currentSAR)
            {
                _isUptrend = true;
                _currentSAR = _extremePrice;
                _extremePrice = newPrice;
                _accelerationFactor = _getStepAF();
            }
            else
            {
                if (newPrice < _extremePrice)
                {
                    _extremePrice = newPrice;
                    _accelerationFactor = Math.Min(_accelerationFactor + _getStepAF(), _maxAF);
                }

                _currentSAR = Math.Max(nextSAR, GetPreviousHigh());
            }
        }
    }


    public double GetValue()
    {
        return _currentSAR;
    }

    public void ResetIndicator()
    {
        _currentSAR = 0.0;
        _extremePrice = 0.0;
        _maxAF = _getStepAF() * 10;
        _isUptrend = true;
        _priceHistory = new Queue<double>();
    }

    public Dictionary<string, object> IntermediateOutputFile()
    {
        return new()
        {
            { "CurrentSAR", _currentSAR },
            { "AccelerationFactor", _accelerationFactor },
            { "IsUptrend", _isUptrend },
            { "ExtremePrice", _extremePrice },
            { "PriceHistory", _priceHistory.ToArray() }
        };
    }

    public void SetIntermediateValues(Dictionary<string, object> intermediateData)
    {
        if (intermediateData.TryGetValue("CurrentSAR", out var currentSARValue) &&
            currentSARValue is double currentSAR)
        {
            _currentSAR = currentSAR;
        }

        if (intermediateData.TryGetValue("AccelerationFactor", out var accelerationFactorValue) &&
            accelerationFactorValue is double accelerationFactor)
        {
            _accelerationFactor = accelerationFactor;
        }

        if (intermediateData.TryGetValue("IsUptrend", out var isUptrendValue) && isUptrendValue is bool isUptrend)
        {
            _isUptrend = isUptrend;
        }

        if (intermediateData.TryGetValue("ExtremePrice", out var extremePriceValue) &&
            extremePriceValue is double extremePrice)
        {
            _extremePrice = extremePrice;
        }

        if (intermediateData.TryGetValue("PriceHistory", out var priceHistoryValue) &&
            priceHistoryValue is JArray priceHistory)
        {
            _priceHistory = new Queue<double>(priceHistory.ToObject<List<double>>()!);
        }
    }
}