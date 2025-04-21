using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.StrategyRuntime;
using Newtonsoft.Json;

namespace FishyLibrary.Models.Strategy;

public class StrategyGet : StrategyBase
{
    public ParametersGet? Parameters { get; set; }
    
    public ICollection<StrategyRuntimeGet> Runtimes { get; set; }
    
    public ICollection<StrategySecondaryProduct.StrategySecondaryProductGet> SecondaryProducts { get; set; }
    public Account.Account Account { get; set; }

    public StrategyGet() : base() { }
    
    public StrategyGet(StrategyGet other) : base(other)
    {
        Parameters = other.Parameters;
    }

    public StrategyGet(int id, 
        string name, 
        string description, 
        string product,
        bool active, 
        string intermediateData, 
        int amountAllocated, 
        bool dry, 
        int numStocksHolding, 
        ParametersGet parameters, 
        DateTime intermediateDataModifiedTime, 
        Account.Account account) : base(id, name, description, product, active, intermediateData, amountAllocated, dry, 
                                numStocksHolding, intermediateDataModifiedTime)
    {
        Parameters = parameters;
        Account = account;
    }
}