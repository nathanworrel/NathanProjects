using AutoMapper;
using FishyLibrary.Models.Trade;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class TradeService : ITradeService
{
    private readonly ILogger<TradeService> _logger;
    private readonly DBAccessContext _dbAccessContext;
    private readonly IMapper _mapper;

    public TradeService(ILogger<TradeService> logger, DBAccessContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbAccessContext = dbContext;
        _mapper = mapper;
    }

    public TradeGet? Get(int id)
    {
        var trade = _dbAccessContext.Trades.Find(id);
        if (trade == null)
        {
            return null;
        }
        return _mapper.Map<TradeGet>(trade);
    }

    public Trade? Find(int id)
    {
        var trade = _dbAccessContext.Trades.Find(id);
        if (trade == null)
        {
            return null;
        }

        return trade;
    }

    public List<TradeGet> GetAll()
    {
        return _dbAccessContext.Trades
            .Include(t => t.Strategy)
            .ThenInclude(s => s.Parameters)
            .ThenInclude(p => p.StrategyType)
            .Select(t => _mapper.Map<TradeGet>(t))
            .ToList();
    }

    public int Add(TradePost trade)
    {
        try
        {
            Trade tradeEntity = _mapper.Map<Trade>(trade);
            tradeEntity.TimePlaced = tradeEntity.TimePlaced.ToUniversalTime();
            tradeEntity.TimeModified = tradeEntity.TimeModified.ToUniversalTime();
            _dbAccessContext.Trades.Add(tradeEntity);
            _dbAccessContext.SaveChanges();
            return tradeEntity.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError("{}", ex);
            return 0;
        }
    }

    public int Edit(Trade trade, TradePost tradePost)
    {
        try
        {
            if (tradePost.StrategyId != trade.StrategyId)
            {
                throw new Exception("Can't change strategy Id");
            }
            trade.TimePlaced = tradePost.TimePlaced.ToUniversalTime();
            trade.TimeModified = tradePost.TimeModified.ToUniversalTime();
            trade.QuantityFilled = tradePost.QuantityFilled;
            trade.QuantityPlaced = tradePost.QuantityPlaced;
            trade.PriceFilled = tradePost.PriceFilled;
            trade.PricePlaced = tradePost.PricePlaced;
            trade.Status = tradePost.Status;
            trade.OrderNumber = tradePost.OrderNumber;
            trade.Side = tradePost.Side;
            _dbAccessContext.Trades.Update(trade);
            _dbAccessContext.SaveChanges();
            return trade.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public TradePost Delete(Trade trade)
    {
        _dbAccessContext.Trades.Remove(trade);
        _dbAccessContext.SaveChanges();
        return _mapper.Map<TradePost>(trade);
    }
}