using MakeTrade.Models;

namespace FishyLibrary.Helpers;

/*
 * Used to store a specific portfolios Account information and functionality
 * - TODO: add user id or user field
 */
public class AccountInfo
{
    public Dictionary<string, Models.Standing> Positions { get; set; }
    
    public Balance? Balance { get; set; }
    
    public string AccountNumber { get; set; }

    public AccountInfo()
    {
        Positions = new Dictionary<string, Models.Standing>();
        Balance = new Balance(0);
        AccountNumber = "";
    }
    
    public AccountInfo(Balance balance)
    {
        Positions = new Dictionary<string, Models.Standing>();
        Balance = balance;
        AccountNumber = "";
    }

    public AccountInfo(Balance balance, Dictionary<string, Models.Standing> positions)
    {
        Positions = positions;
        Balance = balance;
        AccountNumber = "";
    }
    
    public float GetProductAllocation(string product)
    {
        if (Positions.TryGetValue(product, out var position))
        {
            return position.LongQuantity == 0 ? 0 : 1;
        }
        return 0;
    }

    public int GetProductLongQuantity(string product)
    {
        if (Positions.TryGetValue(product, out var position))
        {
            return position.LongQuantity;
        }
        return 0;
    }

    public double GetAvailableCash()
    {
        if (Balance == null)
        {
            return 0;
        }
        else
        {
            return Balance.CashBalance;
        }
    }
}