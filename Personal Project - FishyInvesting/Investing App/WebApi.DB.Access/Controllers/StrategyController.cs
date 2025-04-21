using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class StrategyController : ControllerBase
{
    private readonly ILogger<StrategyController> _logger;
    private readonly IStrategyService _strategyService;

    public StrategyController(ILogger<StrategyController> logger, IStrategyService strategyService)
    {
        _logger = logger;
        _strategyService = strategyService;
    }

    [HttpGet]
    public ActionResult<StrategyGet> Get(int id)
    {
        StrategyGet? strategy = _strategyService.Get(id);
        if (strategy == null)
        {
            return NotFound();
        }
        return strategy;
    }

    [HttpGet("all")]
    public ActionResult<List<StrategyGet>> GetAll()
    {
        return _strategyService.GetAll();
    }

    [HttpGet("account")]
    public ActionResult<List<StrategyGet>> GetUser(int accountId)
    {
        return _strategyService.GetAllForAccount(accountId);
    }

    [HttpPost]
    public ActionResult<StrategyGet> Post(StrategyPost strategy)
    {
        int id = _strategyService.AddStrategy(strategy);
        if (id == 0)
        {
            return BadRequest();
        }
        
        StrategyGet? result = _strategyService.Get(id);
        if (result == null)
        {
            return BadRequest("Failed to update strategy");
        }
        
        return result;
    }

    [HttpPut]
    public ActionResult<StrategyGet> Put(StrategyPost strategy)
    {
        Strategy? strategyEntity = _strategyService.Find(strategy.Id);
        if (strategyEntity == null)
        {
            return NotFound();
        }
        
        int id = _strategyService.EditStrategy(strategyEntity, strategy);
        if (id == 0)
        {
            return BadRequest("Failed to update strategy");
        }
        
        StrategyGet? result = _strategyService.Get(id);
        if (result == null)
        {
            return BadRequest("Failed to update strategy");
        }
        
        return result;
    }

    [HttpDelete]
    public ActionResult<StrategyPost> Delete(int strategyId)
    {
        var strategy = _strategyService.Find(strategyId);
        if (strategy == null)
        {
            return NotFound();
        }
        
        return _strategyService.Delete(strategy);
    }
}