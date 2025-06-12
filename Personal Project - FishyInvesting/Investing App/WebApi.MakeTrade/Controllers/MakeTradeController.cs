using EasyNetQ;
using FishyLibrary;
using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using MakeTrade.Services;
using Microsoft.AspNetCore.Mvc;

namespace MakeTrade.Controllers;

[ApiController]
[Route("[controller]")]
public class MakeTradeController : Controller
{
    private readonly ILogger<MakeTradeController> _logger;
    private readonly IMakeTradeService _service;

    public MakeTradeController(ILogger<MakeTradeController> logger, IMakeTradeService service)
    {
        _logger = logger;
        _service = service;
    }
    
    [HttpPost]
    public IActionResult MakeTrade(float desiredPosition, int strategyId, double currentPrice, bool dry = true)
    {
        try
        {
            _logger.LogInformation("Attempting to make trade for strategy: {strategyId} with desired position " +
                                   "{desiredPosition} at {currentPrice}.", strategyId, desiredPosition, currentPrice);
            Trade? trade = _service.MakeTrade(desiredPosition, strategyId, currentPrice, dry);
            _service.SendNotification(strategyId, dry, trade);
            if (dry)
            {
                return Ok("No Trade Made");
            }
            if (trade == null)
            {
                return BadRequest("Trade Not Found");
            }
            return Ok(trade);
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }
}