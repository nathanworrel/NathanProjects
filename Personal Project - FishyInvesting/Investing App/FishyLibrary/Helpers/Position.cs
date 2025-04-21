using FishyLibrary.Models;
using FishyLibrary.Models.Earning;
using FishyLibrary.Models.Trade;

namespace FishyLibrary.Helpers;

public class Position
{
    record IntervalBuy(decimal ChangeInAllocation, decimal Price);
    public decimal CurrentAllocation { get; private set; }
    private decimal LastPrice { get; set; }
    private Queue<IntervalBuy> IntervalBuys { get; set; }
    private decimal IntervalReturns { get; set; }
    
    public Position()
    {
        CurrentAllocation = 0;
        LastPrice = 0;
        IntervalBuys = new Queue<IntervalBuy>();
        Reset();
    }

    public Position(StockData stockData)
    {
        CurrentAllocation = 0;
        LastPrice = stockData.Price;
        IntervalBuys = new Queue<IntervalBuy>();
        Reset();
    }
    
    public void AddTradeInInterval(Trade trade)
    {            
        if ((Side)trade.Side == Side.SELL)
        {
            decimal percentageSum = 0;
            decimal changeInAllocation = CurrentAllocation - trade.DesiredAllocation;
            
            while (percentageSum < changeInAllocation && IntervalBuys.Count > 0)
            {
                var buy = IntervalBuys.Dequeue();
                if (percentageSum + buy.ChangeInAllocation > changeInAllocation)
                {
                    var remainingAllocation = percentageSum + buy.ChangeInAllocation - trade.DesiredAllocation;
                    IntervalReturns *= CalculatePercentageChange(buy.Price, trade.PriceFilled, buy.ChangeInAllocation - remainingAllocation);
                    IntervalBuys.Enqueue( buy with { ChangeInAllocation = remainingAllocation });
                    percentageSum = trade.DesiredAllocation;
                }
                else
                {
                    percentageSum += buy.ChangeInAllocation;
                    IntervalReturns *= GenerateReturnsForIntervalBuy(buy, trade.PriceFilled);
                }
            }

            if (percentageSum < changeInAllocation)
            {
                IntervalReturns *= CalculatePercentageChange(LastPrice, trade.PriceFilled, changeInAllocation);
            }
        }
        else
        {
            if (CurrentAllocation > trade.DesiredAllocation)
            {
                throw new Exception("Allocation Amount Incorrect");
            }
            IntervalBuys.Enqueue(new IntervalBuy(trade.DesiredAllocation - CurrentAllocation, trade.PriceFilled));
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
        // Generate returns for the buys in this interval without 
        foreach (var intervalBuy in IntervalBuys)
        {
            IntervalReturns *= GenerateReturnsForIntervalBuy(intervalBuy, stockData.Price);
        }
        var finalReturns = GenerateFinalReturns(stockData.Price);
        var result = new Return(stockData.Time, IntervalReturns * finalReturns);
        LastPrice = stockData.Price;
        Reset();
        return result;
    }

    /*
     * Resets the variables for an interval
     */
    private void Reset()
    {
        IntervalReturns = 1;
        IntervalBuys.Clear();
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
        return (percentage * (endingPrice - originalPrice) / originalPrice) + 1;
    }

    private decimal GenerateReturnsForIntervalBuy(IntervalBuy buy, decimal sellPrice)
    {
        return CalculatePercentageChange(buy.Price, sellPrice, buy.ChangeInAllocation);
    }
    
    private decimal GenerateFinalReturns(decimal currPrice)
    {
        decimal changeInAllocation = IntervalBuys.ToList().Sum(b => b.ChangeInAllocation);
        if (CurrentAllocation != changeInAllocation)
        {
            return CalculatePercentageChange(LastPrice, currPrice, CurrentAllocation - changeInAllocation);
        }
    
        return 1;
    }
}