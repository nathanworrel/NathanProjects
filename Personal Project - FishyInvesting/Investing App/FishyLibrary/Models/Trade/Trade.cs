namespace FishyLibrary.Models.Trade;

public class Trade : TradeBase
{
    public int StrategyId { get; set; }
    
    public Strategy.Strategy Strategy { get; set; }
    
    public Trade() {}
    
    public Trade(int id, 
        DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        long orderNumber,
        int status,
        int side,
        DateTime timeModified,
        int quantityFilled,
        decimal priceFilled,
        decimal desiredAllocation,
        int strategyId) : base(id, timePlaced, quantityPlaced, pricePlaced, orderNumber, status, side, timeModified, quantityFilled, priceFilled, desiredAllocation)
    {
        StrategyId = strategyId;
    }
    
    public Trade(
        DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        long orderNumber,
        int status,
        int side,
        DateTime timeModified,
        int quantityFilled,
        decimal priceFilled,
        decimal desiredAllocation,
        int strategyId) : base(timePlaced, quantityPlaced, pricePlaced, orderNumber, status, side, timeModified, quantityFilled, priceFilled, desiredAllocation)
    {
        StrategyId = strategyId;
    }
    
    public Trade(DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        int side, 
        decimal desiredAllocation) : base(timePlaced, quantityPlaced, pricePlaced, side, desiredAllocation)
    {
        StrategyId = 0;
    }
}