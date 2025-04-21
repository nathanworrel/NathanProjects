namespace FishyLibrary.Models.StrategySecondaryProduct;

public class StrategySecondaryProductPost : StrategySecondaryProductBase
{
    public int StrategyId { get; set; }
    
    public StrategySecondaryProductPost() {}

    public StrategySecondaryProductPost(int id, string product, int strategyId, int useOrder) : base(id, product,
        useOrder)
    {
        StrategyId = strategyId;
    }
}