namespace FishyLibrary.Models.Account;

public class AccountPost : AccountBase
{
    public int ClientId { get; set; }
    
    public AccountPost() {}

    public AccountPost(int id, string accountId, string hashAccountId, int clientId) : base(id, accountId, hashAccountId)
    {
        ClientId = clientId;
    }
}