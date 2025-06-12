using Microsoft.AspNetCore.Mvc;
using WebApi.Scheduler.Services;

namespace WebApi.Scheduler.Controllers;

[ApiController]
[Route("[controller]")]
public class SchedulerController : ControllerBase
{
    private readonly ILogger<SchedulerController> _logger;
    private readonly ISchedulerService _schedulerService;

    public SchedulerController(ILogger<SchedulerController> logger, ISchedulerService schedulerService)
    {
        _logger = logger;
        _schedulerService = schedulerService;
    }

    [HttpPost("start")]
    public IActionResult Scheduler(string runTime = "09:50:00")
    {
        _logger.LogInformation("Scheduler started at {runTime}", runTime);
        try
        {
            _schedulerService.StartScheduler(TimeSpan.Parse(runTime));
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }
}