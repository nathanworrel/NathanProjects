using AutoMapper;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Strategies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class ParametersController : ControllerBase
{
    private readonly ILogger<ParametersController> _logger;
    private readonly IParametersService _parametersService;

    public ParametersController(ILogger<ParametersController> logger, IParametersService parametersService)
    {
        _logger = logger;
        _parametersService = parametersService;
    }

    [HttpGet]
    public ActionResult<ParametersGet> Get(int id)
    {
        ParametersGet? parameters = _parametersService.GetByIdWithStrategyType(id);
        if (parameters == null)
        {
            return NotFound();
        }

        return parameters;
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<ParametersGet>> GetAll()
    {
        return _parametersService.GetAll();
    }

    [HttpPost]
    public ActionResult<ParametersGet> Post(ParametersPost parameters)
    {
        int id = _parametersService.AddParameters(parameters);
        return Get(id);
    }

    [HttpGet("withStrategy")]
    public ActionResult<List<ParametersGet>> GetStrategies(int strategyTypeId)
    {
        return _parametersService.GetByStrategyType(strategyTypeId);
    }

    [HttpDelete]
    public ActionResult<ParametersPost> Delete(int id)
    {
        Parameters? parameters = _parametersService.GetById(id);
        if (parameters == null)
        {
            return NotFound();
        }
        var param = _parametersService.DeleteByParameters(parameters);
        if (param == null)
        {
            return BadRequest();
        }
        return param;
    }
}