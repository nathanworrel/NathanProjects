using FishyLibrary.Models.Strategy;

namespace WebApi.DB.Access.Service;

public interface IStrategyService
{
    StrategyGet? Get(int id);
    Strategy? Find(int id);
    List<StrategyGet> GetAll();
    List<StrategyGet> GetAllForAccount(int accountId);
    int AddStrategy(StrategyPost strategy);
    int EditStrategy(Strategy strategyEntity, StrategyPost strategy);
    StrategyPost Delete(Strategy strategy);
}