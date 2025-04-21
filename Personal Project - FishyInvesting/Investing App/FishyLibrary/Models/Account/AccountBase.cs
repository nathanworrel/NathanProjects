namespace FishyLibrary.Models.Account;

public class AccountBase
{
    public int Id { get; set; }
    public string AccountId { get; set; }
    public string HashAccountId { get; set; }
    
    public AccountBase() {}

    public AccountBase(int id, string accountId, string hashAccountId)
    {
        Id = id;
        AccountId = accountId;
        HashAccountId = hashAccountId;
    }
}