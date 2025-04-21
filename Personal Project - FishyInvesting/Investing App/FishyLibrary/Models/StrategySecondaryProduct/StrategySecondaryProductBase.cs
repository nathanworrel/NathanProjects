namespace FishyLibrary.Models.StrategySecondaryProduct;

public class StrategySecondaryProductBase
{
    public int Id { get; set; }
    public string Product { get; set; }
    public int UseOrder { get; set; }
    
    public StrategySecondaryProductBase() {}

    public StrategySecondaryProductBase(int id, string product, int useOrder)
    {
        Id = id;
        Product = product;
        UseOrder = useOrder;
    }
}