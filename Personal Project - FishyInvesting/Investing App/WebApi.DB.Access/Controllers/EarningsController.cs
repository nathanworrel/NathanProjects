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

    public EarningsController(ILogger<EarningsController> logger, DBAccessContext dbContext, IEarningService earningService, IGetDataRetriever getDataRetriever)
    {
        _logger = logger;
        _dbContext = dbContext;
        _earningService = earningService;
        _getDataRetriever = getDataRetriever;
    }

    [HttpGet]
    public ActionResult<Earning> Get(int strategyId, string startDate = "2000-01-01")
    {
        Strategy? strategy = _dbContext.Strategies.FirstOrDefault(s => s.Id == strategyId);
        if (strategy == null)
        {
            return NotFound("Strategy not found");
        }
        
        List<StockData> prices = _getDataRetriever.GetData(startDate, strategy.Product);
        if (!prices.Any())
        {
            return BadRequest("Pricing data not found");
        }
        
        List<Trade> trades = _dbContext.Trades.Where(t => t.StrategyId == strategyId).OrderBy(t => t.TimePlaced).ToList();
        
        List<Return> returns = _earningService.GenerateReturns(prices, trades);
        
        return new Earning(returns, strategyId, _earningService.GetTotalProfits(trades));
    }
}