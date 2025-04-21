using Microsoft.AspNetCore.Mvc;
using WebApi.UpdateTrades.Services;

namespace WebApi.UpdateTrades.Controllers;

[ApiController]
[Route("[controller]")]
public class UpdateTradesController : ControllerBase
{
    private readonly ILogger<UpdateTradesController> _logger;
    private readonly IUpdateTradesService _updateTradesService;

    public UpdateTradesController(ILogger<UpdateTradesController> logger, IUpdateTradesService updateTradesService)
    {
        _logger = logger;
        _updateTradesService = updateTradesService;
    }

    [HttpPatch()]
    public IActionResult UpdateTrades()
    {
        try
        {
            _updateTradesService.UpdateTrades();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
        
    }
}