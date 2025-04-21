using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategySecondaryProduct;

namespace WebApi.DB.Access.Service;

public interface IStrategySecondaryProductService
{
    StrategySecondaryProductGet? Get(int id);
    StrategySecondaryProduct? Find(int id);
    List<StrategySecondaryProductGet> GetAll();
    List<StrategySecondaryProductGet> GetAllByStrategy(int strategyId);
    int Add(StrategySecondaryProductPost secondaryProduct);
    void SetRuntimes(Strategy strategy, List<StrategySecondaryProductPost> strategySecondaryProduct);
    StrategySecondaryProductPost Delete(StrategySecondaryProduct secondary);
}