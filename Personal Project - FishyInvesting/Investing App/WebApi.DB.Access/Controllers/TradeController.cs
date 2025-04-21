using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class TradeController : ControllerBase
{
    private readonly ILogger<TradeController> _logger;
    private readonly ITradeService _tradeService;
    
    public TradeController(ILogger<TradeController> logger, ITradeService tradeService)
    {
        _logger = logger;
        _tradeService = tradeService;
    }

    [HttpGet]
    public ActionResult<TradeGet> Get(int id)
    {
        TradeGet? trade = _tradeService.Get(id);
        if (trade == null)
        {
            return NotFound();
        }

        return trade;
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<TradeGet>> Get()
    {
        return _tradeService.GetAll();
    }

    [HttpPost]
    public ActionResult<TradeGet> Post(TradePost trade)
    {
        int id = _tradeService.Add(trade);
        if (id <= 0)
        {
            return BadRequest();
        }
        TradeGet? tradeEntity = _tradeService.Get(id);
        if (tradeEntity == null)
        {
            return BadRequest();
        }
        return tradeEntity;
    }

    [HttpPut]
    public ActionResult<TradeGet> Put(TradePost trade)
    {
        Trade? tradeEntity = _tradeService.Find(trade.Id);
        if (tradeEntity == null)
        {
            return NotFound();
        }
        
        int id = _tradeService.Edit(tradeEntity, trade);
        TradeGet? result = _tradeService.Get(id);
        if (result == null)
        {
            return BadRequest();
        }
        return result;
    }

    [HttpDelete]
    public ActionResult<TradePost> Delete(int id)
    {
        var trade = _tradeService.Find(id);
        if (trade == null)
        {
            return NotFound();
        }

        return _tradeService.Delete(trade);
    }
}