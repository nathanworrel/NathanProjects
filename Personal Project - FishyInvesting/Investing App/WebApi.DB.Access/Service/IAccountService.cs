using FishyLibrary.Models.Account;
using FishyLibrary.Models.Strategy;

namespace WebApi.DB.Access.Service;

public interface IAccountService
{
    List<AccountGet> GetAllAccounts();
    int AddAccount(AccountPost account);
    Account? Find(int id);
    
    List<Strategy> GetStrategiesForAccount(int accountId);
}