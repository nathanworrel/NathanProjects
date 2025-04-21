namespace FishyLibrary.Models.Strategy;

public class StrategyPost : StrategyBase
{
    public int ParameterId { get; set; }
    public int AccountId { get; set; }
    
    public StrategyPost() : base() { }

    public StrategyPost(Strategy other)
    {
        ParameterId = other.ParameterId;
    }

    public StrategyPost(int id, 
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