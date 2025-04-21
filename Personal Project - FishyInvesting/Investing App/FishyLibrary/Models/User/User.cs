namespace FishyLibrary.Models.User;

public class User : UserBase
{
    public ICollection<Account.Account> Accounts { get; set; }
}