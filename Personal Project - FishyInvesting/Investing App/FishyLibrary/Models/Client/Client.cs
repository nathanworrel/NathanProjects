namespace FishyLibrary.Models.Client;

public class Client : ClientBase
{
    public ICollection<Account.Account> Accounts { get; set; }
}