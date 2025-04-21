using AutoMapper;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategySecondaryProduct;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class StrategySecondaryProductsService : IStrategySecondaryProductService
{
    private readonly ILogger<StrategySecondaryProductsService> _logger;
    private readonly DBAccessContext _dbAccessContext;
    private readonly IMapper _mapper;

    public StrategySecondaryProductsService(ILogger<StrategySecondaryProductsService> logger,
        DBAccessContext dbAccessContext, IMapper mapper)
    {
        _logger = logger;
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
    }

    public StrategySecondaryProductGet? Get(int id)
    {
        var secondary = _dbAccessContext.SecondaryProducts.Find(id);
        if (secondary == null)
        {
            return null;
        }
        return _mapper.Map<StrategySecondaryProductGet>(secondary);
    }

    public StrategySecondaryProduct? Find(int id)
    {
        var secondary = _dbAccessContext.SecondaryProducts.Find(id);
        if (secondary == null)
        {
            return null;
        }

        return secondary;
    }

    public List<StrategySecondaryProductGet> GetAll()
    {
        return _dbAccessContext.SecondaryProducts
            .Select(s => _mapper.Map<StrategySecondaryProductGet>(s))
            .ToList();
    }

    public List<StrategySecondaryProductGet> GetAllByStrategy(int strategyId)
    {
        return _dbAccessContext.SecondaryProducts
            .Where(x => x.StrategyId == strategyId)
            .Select(s => _mapper.Map<StrategySecondaryProductGet>(s))
            .ToList();
    }

    public int Add(StrategySecondaryProductPost secondaryProduct)
    {
        try
        {
            StrategySecondaryProduct secondaryProductEntity = _mapper.Map<StrategySecondaryProduct>(secondaryProduct);
            _dbAccessContext.SecondaryProducts.Add(secondaryProductEntity);
            _dbAccessContext.SaveChanges();
            return secondaryProductEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public void SetRuntimes(Strategy strategy, List<StrategySecondaryProductPost> strategySecondaryProduct)
    {
        List<StrategySecondaryProduct> secondaryProducts = strategySecondaryProduct
            .Select(s => _mapper
                .Map<StrategySecondaryProduct>(s))
            .ToList();
        strategy.SecondaryProducts = secondaryProducts;
        _dbAccessContext.SaveChanges();
    }

    public StrategySecondaryProductPost Delete(StrategySecondaryProduct secondary)
    {
        _dbAccessContext.SecondaryProducts.Remove(secondary);
        _dbAccessContext.SaveChanges();
        return _mapper.Map<StrategySecondaryProductPost>(secondary);
    }
}