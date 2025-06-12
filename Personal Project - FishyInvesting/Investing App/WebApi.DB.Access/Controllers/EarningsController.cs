using CommonServices.Retrievers.GetData;
using FishyLibrary.Models;
using FishyLibrary.Models.Earning;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Trade;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class EarningsController : Controller
{
    private readonly ILogger<EarningsController> _logger;
    private readonly DBAccessContext _dbContext;
    private readonly IEarningService _earningService;
    private readonly IGetDataRetriever _getDataRetriever;
    private readonly IAccountService _accountService;
    private readonly IClientService _clientService;

    public EarningsController(ILogger<EarningsController> logger, DBAccessContext dbContext, 
        IEarningService earningService, IGetDataRetriever getDataRetriever, IAccountService accountService, IClientService clientService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _earningService = earningService;
        _getDataRetriever = getDataRetriever;
        _accountService = accountService;
        _clientService = clientService;
    }

    [HttpGet]
    public ActionResult<Earning> Get(int strategyId, string startDate = "2000-01-01")
    {
        Strategy? strategy = _dbContext.Strategies.FirstOrDefault(s => s.Id == strategyId);
        if (strategy == null)
        {
            return NotFound("Strategy not found");
        }
        
        List<Trade> trades = _dbContext.Trades
            .Where(t => t.StrategyId == strategyId && t.Status == 14)
            .OrderBy(t => t.TimePlaced)
            .ToList();
        
        List<StockData> prices = _getDataRetriever.GetData(startDate, strategy.Product);
        if (!prices.Any())
        {
            return BadRequest("Pricing data not found");
        }
        
        List<Return> returns = _earningService.GenerateReturns(prices, trades);
        
        return new Earning(returns, strategyId, _earningService.GetTotalProfits(trades));
    }

    [HttpGet("market")]
    public ActionResult<Earning> GetMarket(string product, string startDate = "2000-01-01")
    {
        List<StockData> prices = _getDataRetriever.GetData(startDate, product);
        if (!prices.Any())
        {
            return BadRequest("Pricing data not found");
        }

        var firstPrice = prices.OrderBy(x => x.Time).First();
        List<Trade> trades = new List<Trade>()
        {
            new Trade(firstPrice.Time, 1, firstPrice.Price, (int)Side.BUY, 1)
        };
        
        List<Return> returns = _earningService.GenerateReturns(prices, trades);
        
        return new Earning(returns, 0, _earningService.GetTotalProfits(trades));
    }

    [HttpGet("account/{accountId}")]
    public ActionResult<Earning> GetAccountEarnings(int accountId, string startDate = "2000-01-01")
    {
        var strategies = _accountService.GetStrategiesForAccount(accountId);
        if (strategies == null)
        {
            return NotFound("Account not found");
        }

        return GetConsolidatedEarningsForStrategies(strategies, accountId, startDate);
    }
    
    [HttpGet("client/{clientId}")]
    public ActionResult<Earning> GetUserEarnings(int clientId, string startDate = "2000-01-01")
    {
        var accounts = _clientService.GetAccounts(clientId);
        if (accounts == null)
        {
            return NotFound("User not found");
        }
        
        List<Strategy> strategies = new List<Strategy>();
        foreach (var account in accounts)
        {
            strategies.AddRange(_accountService.GetStrategiesForAccount(account.Id));
        }

        return GetConsolidatedEarningsForStrategies(strategies, clientId, startDate);
    }

    private Earning GetConsolidatedEarningsForStrategies(List<Strategy> strategies, int id, string startDate = "2000-01-01")
    {
        Dictionary<string, List<int>> productToStrategies = new Dictionary<string, List<int>>();
        foreach (var strategy in strategies)
        {
            if (productToStrategies.ContainsKey(strategy.Product))
            {
                productToStrategies[strategy.Product].Add(strategy.Id);    
            }
            else
            {
                productToStrategies.Add(strategy.Product, new List<int>() { strategy.Id });
            }
            
        }
        decimal totalProfit = 0;
        
        // turn this into a thread
        var returns = new List<List<Return>>();
        foreach (var productAndStrategy in productToStrategies)
        {
            List<StockData> prices = _getDataRetriever.GetData(startDate, productAndStrategy.Key);
            if (!prices.Any())
            {
                throw new Exception("Pricing data not found");
            }

            foreach (var strategyId in productAndStrategy.Value)
            {
                List<Trade> trades = _dbContext.Trades
                    .Where(t => t.StrategyId == strategyId && t.Status == 14)
                    .OrderBy(t => t.TimePlaced)
                    .ToList();
                returns.Add(_earningService.GenerateReturns(prices, trades));
                totalProfit += _earningService.GetTotalProfits(trades);
            }
        }
        
        return new Earning(_earningService.ConsolidateReturns(returns), id, totalProfit);
    }
}