namespace FishyLibrary.Models.StrategySecondaryProduct;

public class StrategySecondaryProduct : StrategySecondaryProductBase
{
    public int StrategyId { get; set; }
    
    public Strategy.Strategy Strategy { get; set; }
}