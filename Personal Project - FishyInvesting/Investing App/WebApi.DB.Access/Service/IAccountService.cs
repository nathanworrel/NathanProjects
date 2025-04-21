using FishyLibrary.Models.Account;

namespace WebApi.DB.Access.Service;

public interface IAccountService
{
    List<AccountGet> GetAllAccounts();
    int AddAccount(AccountPost account);
}