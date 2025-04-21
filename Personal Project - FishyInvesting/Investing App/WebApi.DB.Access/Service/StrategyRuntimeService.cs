using AutoMapper;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class StrategyRuntimeService : IStrategyRuntimeService
{
    private ILogger<StrategyRuntimeService> _logger;
    private DBAccessContext _dbAccessContext;
    private readonly IMapper _mapper;

    public StrategyRuntimeService(ILogger<StrategyRuntimeService> logger, DBAccessContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbAccessContext = dbContext;
        _mapper = mapper;
    }

    public StrategyRuntimeGet? Get(int id)
    {
        var runtime = _dbAccessContext.StrategyRuntimes.FirstOrDefault(x => x.Id == id);
        if (runtime == null)
        {
            return null;
        }
        return _mapper.Map<StrategyRuntimeGet>(runtime);
    }
    
    public StrategyRuntime? Find(int id)
    {
        var runtime = _dbAccessContext.StrategyRuntimes.FirstOrDefault(x => x.Id == id);
        if (runtime == null)
        {
            return null;
        }

        return runtime;
    }

    public List<StrategyRuntimeGet> GetAll()
    {
        return _dbAccessContext.StrategyRuntimes.
            Select(sr => _mapper.Map<StrategyRuntimeGet>(sr))
            .ToList();
    }

    public List<StrategyRuntimeGet> GetAllByStrategy(int strategyId)
    {
        return _dbAccessContext.StrategyRuntimes
            .Where(x => x.StrategyId == strategyId)
            .Select(sr => _mapper.Map<StrategyRuntimeGet>(sr))
            .ToList();
    }

    public int Add(int strategyId, string time = "9:30:00")
    {
        try
        {
            StrategyRuntime sr = new StrategyRuntime(0, strategyId, TimeSpan.Parse(time));
            _dbAccessContext.StrategyRuntimes.Add(sr);
            _dbAccessContext.SaveChanges();
            return sr.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError("{}", ex);
            return 0;
        }
    }

    public void SetRuntimes(Strategy strategy, List<StrategyRuntimeString> times)
    {
        List<StrategyRuntime> strategyRuntimes = times
            .Select(t => new StrategyRuntime(t.Id, strategy.Id, TimeSpan.Parse(t.Runtime)))
            .ToList();
        strategy.Runtimes = strategyRuntimes;
        _dbAccessContext.SaveChanges();
    }

    public StrategyRuntimePost Delete(StrategyRuntime strategyRuntime)
    {
        _dbAccessContext.StrategyRuntimes.Remove(strategyRuntime);
        _dbAccessContext.SaveChanges();
        return _mapper.Map<StrategyRuntimePost>(strategyRuntime);
    }
}