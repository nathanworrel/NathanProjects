namespace FishyLibrary.Models.Trade;

public class TradePost : TradeBase
{
    public int StrategyId { get; set; }
    
    public TradePost(DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        int side,
        decimal desiredAllocation,
        int strategyId) : base(timePlaced, quantityPlaced, pricePlaced, side, desiredAllocation)
    {
        StrategyId = strategyId;
    }
    
    public TradePost(DateTime timePlaced,
        DateTime timeUpdated,
        int quantityPlaced,
        decimal pricePlaced,
        int quantityFilled,
        decimal priceFilled,
        int status,
        int orderNumber,
        int side,
        int strategyId)
    {
        Id = 0;
        TimePlaced = timePlaced;
        QuantityPlaced = quantityPlaced;
        PricePlaced = pricePlaced;
        OrderNumber = orderNumber;
        Status = status;
        Side = side;
        TimeModified = timeUpdated;
        QuantityFilled = quantityFilled;
        PriceFilled = priceFilled;
        StrategyId = strategyId;
    }
}