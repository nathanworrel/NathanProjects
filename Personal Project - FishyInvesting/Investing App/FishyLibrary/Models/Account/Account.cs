namespace FishyLibrary.Models.Account;

public class Account : AccountBase
{
    public int UserId { get; set; }
    public User.User User { get; set; }
    
    public ICollection<Strategy.Strategy> Strategies { get; set; }
}