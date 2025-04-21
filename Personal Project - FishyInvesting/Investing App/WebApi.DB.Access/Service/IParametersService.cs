using FishyLibrary.Models.Parameters;

namespace WebApi.DB.Access.Service;

public interface IParametersService
{
    ParametersGet? GetByIdWithStrategyType(int id);
    List<ParametersGet> GetAll();
    Parameters? GetById(int id);
    int AddParameters(ParametersPost param);
    List<ParametersGet> GetByStrategyType(int strategyTypeId);
    ParametersPost? DeleteByParameters(Parameters parameters);
}