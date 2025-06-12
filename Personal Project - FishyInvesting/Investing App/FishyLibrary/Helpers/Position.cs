using FishyLibrary.Models;
using FishyLibrary.Models.Trade;

namespace FishyLibrary.Helpers;

public class Position
{
    private decimal IntervalReturns { get; set; }
    private int NumStocksHolding { get; set; }
    private decimal AverageBuyPrice { get; set; }
    public decimal CurrentAllocation { get; set; }
    private decimal TotalProfits { get; set; }
    
    public Position()
    {
        TotalProfits = 0;
        Reset(0);
    }

    public Position(StockData stockData)
    {
        TotalProfits = 0;
        Reset(stockData.Price);
    }
    
    /*
     * Adds a trade to the interval. Calculates the percentage change using buy and sell price
     * and updates whether in the market or not.
     */
    public void AddTradeInInterval(Trade trade)
    {            
        if ((Side)trade.Side == Side.SELL)
        {
            decimal changeInAllocation = CurrentAllocation - trade.DesiredAllocation;
            int intervalStocksUsed = trade.QuantityFilled;
            UpdateProfits(AverageBuyPrice, trade.PriceFilled, intervalStocksUsed);
            IntervalReturns *= CalculatePercentageChange(AverageBuyPrice, trade.PriceFilled, 
                changeInAllocation);
            NumStocksHolding -= intervalStocksUsed;
        }
        else
        {
            AverageBuyPrice = (AverageBuyPrice * NumStocksHolding + trade.PriceFilled * trade.QuantityFilled) /
                                      (NumStocksHolding +  trade.QuantityFilled);
            NumStocksHolding += trade.QuantityFilled;
        }

        CurrentAllocation = trade.DesiredAllocation;
    }

    /*
     * Adds trades to the current interval
     */
    public void AddAllTradesInInterval(List<Trade> trades)
    {
        foreach (var trade in trades)
        {
            AddTradeInInterval(trade);
        }
    }

    /*
     * Increases the interval
     *  stockData - StockData that is the end of the previous interval and start of the next
     * 
     *  returns the percentage return of the now previous interval
     */
    public Return IncreaseInterval(StockData stockData)
    {
        // Generate returns for stocks currently held
        IntervalReturns *= CalculatePercentageChange(AverageBuyPrice, stockData.Price, CurrentAllocation);
        
        // Generate Result
        var result = new Return(stockData.Time, IntervalReturns, stockData.Price * NumStocksHolding + TotalProfits);
        Reset(stockData.Price);
        return result;
    }

    /*
     * Resets the variables for an interval
     */
    private void Reset(decimal buyPrice)
    {
        IntervalReturns = 1;
        AverageBuyPrice = buyPrice;
    }

    /*
     * Calculates the percentage change for a proportion of an amount
     *  originalPrice - the starting price (buy)
     *  endingPrice - the ending price (sell)
     *  percentage - the amount of money invested in this trade
     *
     * returns a decimal of the percentage change. If no change, 1.
     */
    private decimal CalculatePercentageChange(decimal originalPrice, decimal endingPrice, decimal percentage)
    {
        if (originalPrice != 0)
        {
            return (percentage * (endingPrice - originalPrice) / originalPrice) + 1;
        }
        else
        {
            return 1;
        }
    }

    private void UpdateProfits(decimal buyPrice, decimal sellPrice, int quantity)
    {
        TotalProfits += (sellPrice - buyPrice) * quantity;
    }
}