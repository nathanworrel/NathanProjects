using FishyLibrary.Models;
using FishyLibrary.Models.StrategyType;

namespace WebApi.DB.Access.Service;

public interface IStrategyTypeService
{
    StrategyTypeGet? GetById(int id);
    List<StrategyTypeGet> GetAll();
    
    int AddStrategyType(StrategyTypePost st);
}