namespace FishyLibrary.Models.Account;

public class AccountPost : AccountBase
{
    public int UserId { get; set; }
    
    public AccountPost() {}

    public AccountPost(int id, string accountId, string hashAccountId, int userId) : base(id, accountId, hashAccountId)
    {
        UserId = userId;
    }
}