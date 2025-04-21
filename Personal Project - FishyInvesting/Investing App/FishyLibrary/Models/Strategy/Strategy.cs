using FishyLibrary.Helpers;
using Newtonsoft.Json;

namespace FishyLibrary.Models.Strategy;

public class Strategy : StrategyBase
{
    public int ParameterId { get; set; }
    public Parameters.Parameters? Parameters { get; set; }
    public ICollection<StrategyRuntime.StrategyRuntime> Runtimes { get; set; }
    public ICollection<StrategySecondaryProduct.StrategySecondaryProduct> SecondaryProducts { get; set; }
    [JsonIgnore]
    public ICollection<Trade.Trade> Trades { get; set; }
    public int AccountId { get; set; }
    public Account.Account Account { get; set; }
    
    public Strategy() : base() { }

    public Strategy(Strategy other)
    {
        ParameterId = other.ParameterId;
    }

    public Strategy(int id, 
        string name, 
        string description, 
        string product, 
        bool active, 
        string intermediateData, 
        int amountAllocated, 
        bool dry, 
        int numStocksHolding, 
        int parameterId, 
        DateTime intermediateDataModifiedTime, 
        int accountId) : base(id, name, description, product, active, intermediateData, amountAllocated, dry, 
        numStocksHolding, intermediateDataModifiedTime)
    {
        ParameterId = parameterId;
        AccountId = accountId;
    }
    
}