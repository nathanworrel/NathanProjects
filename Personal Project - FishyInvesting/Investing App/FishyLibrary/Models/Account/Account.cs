namespace FishyLibrary.Models.Account;

public class Account : AccountBase
{
    public int ClientId { get; set; }
    public Client.Client Client { get; set; }
    
    public ICollection<Strategy.Strategy> Strategies { get; set; }
}