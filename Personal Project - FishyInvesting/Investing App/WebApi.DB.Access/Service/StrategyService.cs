using AutoMapper;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class StrategyService : IStrategyService
{
    private readonly ILogger<StrategyService> _logger;
    private readonly DBAccessContext _dbAccessContext;
    private readonly IMapper _mapper;

    public StrategyService(DBAccessContext dbAccessContext, IMapper mapper, ILogger<StrategyService> logger)
    {
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
        _logger = logger;
    }
    
    private IIncludableQueryable<Strategy, ICollection<StrategyRuntime>> GetStrategy()
    {
        return _dbAccessContext.Strategies
            .Include(strategy => strategy.Parameters)
            .ThenInclude(param => param.StrategyType)
            .Include(s => s.SecondaryProducts)
            .Include(s => s.Runtimes);
    }

    public StrategyGet? Get(int id)
    {
        Strategy? strategy = GetStrategy()
            .FirstOrDefault(strategy => strategy.Id == id);
        if (strategy == null)
        {
            return null;
        }
        return _mapper.Map<StrategyGet>(strategy);
    }

    public Strategy? Find(int id)
    {
        Strategy? strategy = _dbAccessContext.Strategies
            .FirstOrDefault(strategy => strategy.Id == id);
        if (strategy == null)
        {
            return null;
        }

        return strategy;
    }

    public List<StrategyGet> GetAll()
    {
        return GetStrategy()
            .Select(s => _mapper.Map<StrategyGet>(s))
            .ToList();
    }

    public List<StrategyGet> GetAllForAccount(int accountId)
    {
        return GetStrategy()
            .Where(x=>x.AccountId == accountId)
            .Select(s => _mapper.Map<StrategyGet>(s))
            .ToList();
    }

    public int AddStrategy(StrategyPost strategy)
    {
        try
        {
            var strategyEntity = _mapper.Map<Strategy>(strategy);
            strategyEntity.IntermediateDataModifiedTime = DateTime.MinValue;
            strategyEntity.IntermediateData = new Dictionary<string, object>();
            if (strategyEntity.AmountAllocated < 0)
            {
                throw new Exception("Negative Amount Allocated not allowed");
            }
            _dbAccessContext.Strategies.Add(strategyEntity);
            _dbAccessContext.SaveChanges();
            return strategyEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public int EditStrategy(Strategy strategyEntity, StrategyPost strategy)
    {
        try
        {
            if (_dbAccessContext.Trades.Any(t => t.StrategyId == strategy.Id))
            {
                if (strategyEntity.Parameters.Id != strategy.ParameterId)
                {
                    throw new Exception("Can't change Parameters");
                }

                if (strategyEntity.Product != strategy.Product)
                {
                    throw new Exception("Can't change Product");
                }
            }

            if (strategy.AmountAllocated < 0)
            {
                throw new Exception("Negative Amount Allocated not allowed");
            }
        
            strategyEntity.Active = strategy.Active;
            strategyEntity.Dry = strategy.Dry;
            strategyEntity.AmountAllocated = strategy.AmountAllocated;
            strategyEntity.Name = strategy.Name;
            strategyEntity.Description = strategy.Description;
        
            _dbAccessContext.Strategies.Update(strategyEntity);
            _dbAccessContext.SaveChanges();
            return strategyEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public StrategyPost Delete(Strategy strategy)
    {
        _dbAccessContext.Strategies.Remove(strategy);
        _dbAccessContext.SaveChanges();
        return _mapper.Map<StrategyPost>(strategy);
    }
}