using CommonServices.Retrievers.CharlesSchwab;
using CommonServices.Retrievers.GetData;
using CommonServices.Retrievers.MakeTrade;
using CommonServices.Services;
using FishyLibrary.Models;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyType;
using FishyLibrary.Strategies;
using WebApi.StrategyService.Contexts;

namespace WebApi.StrategyService.Services;

public class StrategyServiceService : IStrategyServiceService
{
    private readonly ILogger<StrategyServiceService> _logger;
    private readonly StrategyServiceContext _strategyServiceContext;
    private readonly ICharlesSchwabRetriever _charlesSchwabRetriever;
    private readonly IMakeTradeRetriever _makeTradeRetriever;
    private IStrategy _strategy;
    private Strategy? _strategyInfo;
    private Account? _account;
    private readonly IStrategyHelperService _strategyHelperService;
    private readonly IGetDataRetriever _getDataRetriever;

    public StrategyServiceService(ILogger<StrategyServiceService> logger, StrategyServiceContext strategyServiceContext,
        IStrategyHelperService strategyHelperService, IGetDataRetriever getDataRetriever,
        IMakeTradeRetriever makeTradeRetriever, ICharlesSchwabRetriever charlesSchwabRetriever)
    {
        _logger = logger;
        _strategyServiceContext = strategyServiceContext;
        _strategyHelperService = strategyHelperService;
        _getDataRetriever = getDataRetriever;
        _makeTradeRetriever = makeTradeRetriever;
        _charlesSchwabRetriever = charlesSchwabRetriever;
    }

    public void StrategyRun(int strategyId, TimeSpan runTime)
    {
        _strategyInfo = _strategyServiceContext.Strategies.FirstOrDefault(x => x.Id == strategyId);
        if (_strategyInfo == null)
        {
            throw new Exception($"Can't find strategy with id: {strategyId}");
        }

        _account = _strategyServiceContext.Accounts.FirstOrDefault(x => x.Id == _strategyInfo.AccountId);
        if (_account == null)
        {
            throw new Exception($"Can't find account with id: {_strategyInfo.AccountId}");
        }

        Parameters? parameters =
            _strategyServiceContext.Parameters.FirstOrDefault(x => x.Id == _strategyInfo.ParameterId);
        if (parameters == null)
        {
            throw new Exception($"Can't find parameter with id: {_strategyInfo.ParameterId}");
        }

        StrategyType? strategyType =
            _strategyServiceContext.StrategyTypes.FirstOrDefault(x => x.Id == parameters.StrategyTypeId);
        if (strategyType == null)
        {
            throw new Exception($"Can't find strategy type with id: {parameters.StrategyTypeId}");
        }

        List<string> secondaryProducts =
            _strategyServiceContext.SecondaryProducts.Where(x => x.StrategyId == _strategyInfo.Id)
                .OrderBy(x => x.UseOrder)
                .Select(x => x.Product)
                .ToList();

        List<string> products =
        [
            _strategyInfo.Product
        ];
        products.AddRange(secondaryProducts);

        _strategy = _strategyHelperService.GetStrategy(strategyType.Name);

        if (_strategy == null)
        {
            throw new Exception($"Can't find strategy with name: {_strategyInfo.Name}");
        }

        _strategy.ResetStrategy(parameters);

        UpdateStrategy(runTime, products);

        List<double> prices = _charlesSchwabRetriever.GetCurrentData(products, _account.Id);

        for (var i = 0; i < products.Count; i++)
        {
            _logger.LogInformation($"Product {i + 1}: {products[i]} and price : {prices[i]}");
        }

        double signal = _strategy.GenerateSignal(prices);
        _logger.LogInformation($"Signal: {signal} for strategy {_strategyInfo.Id}");

        _makeTradeRetriever.Trade(signal, prices[0], _strategyInfo.Id, _strategyInfo.Dry);

        var strategyInfoIntermediateData = _strategy.IntermediateOutputFile();
        _strategyInfo.IntermediateData = strategyInfoIntermediateData;
        _strategyInfo.IntermediateDataModifiedTime = DateTime.Now.ToUniversalTime();
        _strategyServiceContext.SaveChanges();
    }

    private void UpdateStrategy(TimeSpan runTime, List<string> products)
    {
        List<List<StockData>> prices;
        if (_strategyInfo!.IntermediateDataModifiedTime == DateTime.MinValue)
        {
            prices = _getDataRetriever.GetBacklogDataNum(runTime, products, (int)_strategy.MaxPeriod);
        }
        else
        {
            _strategy.SetIntermediateValues(_strategyInfo.IntermediateData);
            prices = _getDataRetriever.GetBacklogData(runTime, _strategyInfo!.IntermediateDataModifiedTime, products);
        }
        
        if (prices.Count == 0)
        {
            return;
        }

        _logger.LogInformation($"Updating Strategy {_strategyInfo.Id} with {prices[0].Count} days of information from {prices.Count} sources");
        
        for (int i = 0; i < prices[0].Count; i++)
        {
            List<double> hold = prices.Select(d => (double)d[i].Price).ToList();
            _strategy.GenerateSignal(hold);
        }
    }
}