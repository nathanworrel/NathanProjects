using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.StrategyType;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class StrategyTypeController : ControllerBase
{
    private readonly ILogger<StrategyTypeController> _logger;
    private readonly IStrategyTypeService _service;

    public StrategyTypeController(ILogger<StrategyTypeController> logger, IStrategyTypeService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public ActionResult<StrategyTypeGet?> Get(int id)
    {
        _logger.LogInformation("Get Strategy Type with id {id}", id);
        StrategyTypeGet? strategyType = _service.GetById(id);
        if (strategyType == null)
        {
            return NotFound();
        }

        return strategyType;
    }
    
    [HttpGet("all")]
    public ActionResult<List<StrategyTypeGet>> Get()
    {
        _logger.LogInformation("Get all strategy types");
        return _service.GetAll();
    }
}