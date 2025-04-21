using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FishyLibrary.Models;

public class StrategyBase
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Product { get; set; }
    public bool Active { get; set; }
    public Dictionary<string, object> IntermediateData { get; set; }
    public DateTime IntermediateDataModifiedTime { get; set; }
    public decimal AmountAllocated { get; set; }
    public bool Dry { get; set; }
    public int NumStocksHolding { get; set; }

    public StrategyBase()
    {
        IntermediateData = new Dictionary<string, object>();
    }

    public StrategyBase(StrategyBase other)
    {
        Id = other.Id;
        Name = other.Name;
        Description = other.Description;
        Product = other.Product;
        Active = other.Active;
        IntermediateData = new Dictionary<string, object>(other.IntermediateData);
        IntermediateDataModifiedTime = other.IntermediateDataModifiedTime;
        AmountAllocated = other.AmountAllocated;
        Dry = other.Dry;
        NumStocksHolding = other.NumStocksHolding;
    }
    
    public StrategyBase(int id, 
        string name, 
        string description, 
        string product, 
        bool active, 
        string intermediateData, 
        int amountAllocated, 
        bool dry, 
        int numStocksHolding, 
        DateTime intermediateDataModifiedTime)
    {
        Id = id;
        Name = name;
        Description = description;
        Product = product;
        Active = active;
        AmountAllocated = amountAllocated;
        Dry = dry;
        NumStocksHolding = numStocksHolding;
        IntermediateDataModifiedTime = intermediateDataModifiedTime;
        try
        {
            IntermediateData = JsonConvert.DeserializeObject<Dictionary<string, object>>(intermediateData);
        }
        catch (Exception e)
        {
            IntermediateData = new Dictionary<string, object>();
            IntermediateDataModifiedTime = DateTime.MinValue;
        }
    }
}