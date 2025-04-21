using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.StrategyType;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Controllers;

namespace WebApi.DB.Access.Service;

public class StrategyTypeService : IStrategyTypeService
{
    private readonly DBAccessContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<StrategyTypeService> _logger;

    public StrategyTypeService(DBAccessContext dbContext, IMapper mapper, ILogger<StrategyTypeService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
    }

    public StrategyTypeGet? GetById(int id)
    {
        StrategyType? strategyType = _dbContext.StrategyTypes.Find(id);
        if (strategyType == null)
        {
            return null;
        }
        return _mapper.Map<StrategyTypeGet>(strategyType);
    }

    public List<StrategyTypeGet> GetAll()
    {
        return _dbContext.StrategyTypes.Select(st => _mapper.Map<StrategyTypeGet>(st)).ToList();
    }

    public int AddStrategyType(StrategyTypePost st)
    {
        StrategyType? strategyType = _mapper.Map<StrategyType>(st);
        _dbContext.StrategyTypes.Add(strategyType);
        _dbContext.SaveChanges();
        return strategyType.Id;
    }
}