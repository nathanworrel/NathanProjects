using Microsoft.AspNetCore.Mvc;
using WebApi.StrategyService.Services;

namespace WebApi.StrategyService.Controllers;

[ApiController]
[Route("[controller]")]
public class StrategyServiceController : ControllerBase
{
    private readonly ILogger<StrategyServiceController> _logger;
    private readonly IStrategyServiceService _strategyServiceService;

    public StrategyServiceController(ILogger<StrategyServiceController> logger, IStrategyServiceService strategyServiceService)
    {
        _logger = logger;
        _strategyServiceService = strategyServiceService;
    }

    [HttpPut("StrategyRun")]
    public IActionResult StrategyRun(int strategyId, String runTime = "09:50:00")
    {
        _logger.LogInformation("Executing Strategy Run with id: {strategyId} and run time: {runTime}", strategyId, runTime);
        try
        {
            _strategyServiceService.StrategyRun(strategyId, TimeSpan.Parse(runTime));
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }
}