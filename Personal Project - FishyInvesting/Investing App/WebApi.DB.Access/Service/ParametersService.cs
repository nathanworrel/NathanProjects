using AutoMapper;
using FishyLibrary.Models.Parameters;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class ParametersService : IParametersService
{
    private readonly DBAccessContext _dbAccessContext;
    private readonly IMapper _mapper;
    private readonly ILogger<ParametersService> _logger;

    public ParametersService(DBAccessContext dbAccessContext, IMapper mapper, ILogger<ParametersService> logger)
    {
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
        _logger = logger;
    }
    
    public ParametersGet? GetByIdWithStrategyType(int id)
    {
        Parameters? parameters = _dbAccessContext.Parameters
            .Include(p => p.StrategyType)
            .FirstOrDefault(p => p.Id == id);
        if (parameters == null)
        {
            return null;
        }
        return _mapper.Map<ParametersGet>(parameters);
    }
    
    public Parameters? GetById(int id)
    {
        Parameters? parameters = _dbAccessContext.Parameters
            .Include(p => p.StrategyType)
            .Include(p => p.Strategies)
            .FirstOrDefault(p => p.Id == id);
        return parameters;
    }

    public List<ParametersGet> GetAll()
    {
        return _dbAccessContext.Parameters.Include(p => p.StrategyType)
            .Select(p => _mapper.Map<ParametersGet>(p)).ToList();
    }

    public int AddParameters(ParametersPost param)
    {
        try
        {
            Parameters dbParams = _mapper.Map<Parameters>(param);

            if (dbParams.Params is null || dbParams.Params.Count == 0)
            {
                throw new Exception("Parameters are null");
            }
            
            _dbAccessContext.Parameters.Add(dbParams);
            _dbAccessContext.SaveChanges();
            return dbParams.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError("{}", ex);
            return 0;
        }
    }

    public List<ParametersGet> GetByStrategyType(int strategyTypeId)
    {
        var parameters = _dbAccessContext.Parameters
            .Select(x => x)
            .Where(s => s.StrategyTypeId == strategyTypeId);
        return parameters.Select(p => _mapper.Map<ParametersGet>(p)).ToList();
    }

    public ParametersPost? DeleteByParameters(Parameters parameters)
    {
        try
        {
            _dbAccessContext.Parameters.Remove(parameters);
            _dbAccessContext.SaveChanges();
            return _mapper.Map<ParametersPost>(parameters);
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return null;
        }
    }
}