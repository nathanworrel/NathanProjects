using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategySecondaryProduct;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class StrategySecondaryProductsController : ControllerBase
{
    private readonly ILogger<StrategySecondaryProductsController> _logger;
    private readonly IStrategySecondaryProductService _secondaryProductService;
    private readonly IStrategyService _strategyService;

    public StrategySecondaryProductsController(ILogger<StrategySecondaryProductsController> logger, 
        IStrategySecondaryProductService secondaryProductService,
        IStrategyService strategyService)
    {
        _logger = logger;
        _secondaryProductService = secondaryProductService;
        _strategyService = strategyService;
    }

    [HttpGet]
    public ActionResult<StrategySecondaryProductGet> Get(int id)
    {
        var secondary = _secondaryProductService.Get(id);
        if (secondary == null)
        {
            return NotFound();
        }
        return secondary;
    }

    [HttpGet("all")]
    public ActionResult<List<StrategySecondaryProductGet>> GetAll()
    {
        return _secondaryProductService.GetAll();
    }

    [HttpGet("by_strategy")]
    public ActionResult<List<StrategySecondaryProductGet>> GetByStrategy(int strategyId)
    {
        var strategy = _strategyService.Find(strategyId);
        if (strategy == null)
        {
            return NotFound();
        }

        return _secondaryProductService.GetAllByStrategy(strategyId);
    }

    [HttpPost]
    public ActionResult<StrategySecondaryProductGet> Post(StrategySecondaryProductPost strategySecondaryProduct)
    {
        var strategy = _strategyService.Find(strategySecondaryProduct.StrategyId);
        if (strategy == null)
        {
            return NotFound("Strategy not found");
        }
        
        StrategySecondaryProduct? secondary = _secondaryProductService.Find(strategySecondaryProduct.Id);
        if (secondary != null)
        {
            return BadRequest("Already exists");
        }
        int id = _secondaryProductService.Add(strategySecondaryProduct);
        
        var secondaryProductGet = _secondaryProductService.Get(id);
        if (secondaryProductGet == null)
        {
            return BadRequest();
        }
        return secondaryProductGet;
    }

    [HttpPost("set")]
    public ActionResult<List<StrategySecondaryProductGet>> Set(int strategyId, List<StrategySecondaryProductPost> strategySecondaryProduct)
    {
        var strategy = _strategyService.Find(strategyId);
        if (strategy == null)
        {
            return NotFound();
        }
        _secondaryProductService.SetRuntimes(strategy, strategySecondaryProduct);
        return _secondaryProductService.GetAll();
    }

    [HttpDelete]
    public ActionResult<StrategySecondaryProductPost> Delete(int id)
    {
        var secondary = _secondaryProductService.Find(id);
        if (secondary == null)
        {
            return NotFound();
        }
        return _secondaryProductService.Delete(secondary);
    }
}