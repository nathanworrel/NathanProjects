using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class StrategyRuntimesController : ControllerBase
{
    private readonly ILogger<StrategyRuntimesController> _logger;
    private readonly IStrategyRuntimeService _strategyRuntimeService;
    private readonly IStrategyService _strategyService;

    public StrategyRuntimesController(ILogger<StrategyRuntimesController> logger, 
        IStrategyRuntimeService strategyRuntimeService, IStrategyService strategyService)
    {
        _logger = logger;
        _strategyRuntimeService = strategyRuntimeService;
        _strategyService = strategyService;
    }

    [HttpGet]
    public ActionResult<StrategyRuntimeGet> Get(int id)
    {
        StrategyRuntimeGet? runtime = _strategyRuntimeService.Get(id);
        if (runtime == null)
        {
            return NotFound();
        }

        return runtime;
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<StrategyRuntimeGet>> GetAll()
    {
        return _strategyRuntimeService.GetAll();
    }

    [HttpGet("by_strategy")]
    public ActionResult<IEnumerable<StrategyRuntimeGet>> GetByStrategy(int strategyId)
    {
        var strategy = _strategyService.Find(strategyId);
        if (strategy == null)
        {
            return NotFound();
        }
        return _strategyRuntimeService.GetAllByStrategy(strategyId);
    }

    [HttpPost]
    public ActionResult<StrategyRuntimeGet> Post(int strategyId, string time = "9:30:00")
    {
        int id = _strategyRuntimeService.Add(strategyId, time);
        if (id == 0)
        {
            return BadRequest();
        }

        return _strategyRuntimeService.Get(id);
    }

    [HttpPost("set")]
    public ActionResult<List<StrategyRuntimeGet>> SetRuntimes(int strategyId, List<StrategyRuntimeString> times)
    {
        Strategy? strategy = _strategyService.Find(strategyId);
        if (strategy == null)
        {
            return NotFound();
        }

        try
        {
            _strategyRuntimeService.SetRuntimes(strategy, times);
            return _strategyRuntimeService.GetAll();
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpDelete]
    public ActionResult<StrategyRuntimePost> Delete(int runtimeId)
    {
        var runtime = _strategyRuntimeService.Find(runtimeId);
        if (runtime == null)
        {
            return NotFound();
        }
        return _strategyRuntimeService.Delete(runtime);
    }
}
