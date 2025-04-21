using FishyLibrary.Models;
using FishyLibrary.Helpers;
using FishyLibrary.Strategies;

namespace FishyLibrary.Backtest;

public class BacktestBuilder
{
    private readonly BacktestRunner _backtestRunner;
    
    public BacktestBuilder(IStrategy strategyIn, List<List<StockData>> primaryPricingData)
    {
        _backtestRunner = new BacktestRunner(strategyIn, primaryPricingData);
    }

    public void AddRangedParameter(RangedParameter param)
    {
        if (!_backtestRunner.RangedParameters.TryAdd(param.Name, param))
        {
            throw new NotImplementedException();
        }
    }

    public BacktestRunner Build()
    {
        return _backtestRunner;
    }
}