using FishyLibrary.Models.Account;
using FishyLibrary.Models.Client;

namespace WebApi.DB.Access.Service;

public interface IClientService
{
    int AddUser(ClientPost client);
    List<Account>? GetAccounts(int userId);
    Client? Find(int id);
    List<Client> GetAll();
}