using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using FishyLibrary.Utils;

namespace WebApi.DB.Access.Service;

public class EarningService: IEarningService
{
    public List<Return> GenerateReturns(List<StockData> prices, List<Trade> trades)
    {
        List<Return> returns = new List<Return>();
        Position position = new Position();
        for (int i = 0; i < prices.Count; i++) 
        {
            var price = prices[i];
            // Get next day data
            StockData? nextPrice = null;
            if (i + 1 < prices.Count)
            {
                nextPrice = prices[i + 1];
            }
            // Get trades
            List<Trade> trade = new List<Trade>();
            if (i == 0)
            {
                var temp = trades.Where(t => t.TimePlaced < price.Time).OrderBy(t => t.TimePlaced).ToList();
                if (temp.Count > 0 && temp.Last().Side == (int) Side.BUY)
                {
                    position.AddTradeInInterval(temp.Last());
                }
            }
            trade = trades
                .Where(t => t.TimePlaced >= price.Time && 
                            (nextPrice is null || nextPrice.Time > t.TimePlaced))
                .ToList();
            returns.Add(position.IncreaseInterval(price));
            position.AddAllTradesInInterval(trade);        
        }
        return returns;
    }

    public decimal GetTotalProfits(List<Trade> trades)
    {
        decimal totalProfits = 0;
        decimal amountInvested = 0;
        decimal numStocksHolding = 0;

        foreach (var trade in trades)
        {
            if (trade.Side == (int) Side.SELL && numStocksHolding != 0)
            {
                decimal avgBuyPrice = amountInvested / numStocksHolding;
                decimal buyPrice = trade.QuantityFilled * avgBuyPrice;
                decimal sellPrice = trade.QuantityFilled * trade.PriceFilled;
                totalProfits += (sellPrice - buyPrice);
                numStocksHolding -= trade.QuantityFilled;
                amountInvested -= trade.QuantityFilled * avgBuyPrice;
            }
            else if ( trade.Side ==(int) Side.BUY)
            {
                numStocksHolding += trade.QuantityFilled;
                amountInvested += trade.QuantityFilled * trade.PriceFilled;
            }
        }

        return totalProfits;
    }

    public List<Return> ConsolidateReturns(List<List<Return>> returns)
    {
        int length = returns[0].Count;
        List<Return> finalReturns = new List<Return>();
        
        for (int i = 0; i < length; i++)
        {
            decimal sumStrategyValues = returns.Sum(innerList => innerList[i].Value);
            decimal weightedSumStrategyValues = returns.Sum(innerList => innerList[i].Value * innerList[i].Returns);
            if (sumStrategyValues == 0)
            {
                finalReturns.Add(new Return(returns[0][i].Date, 1, sumStrategyValues));
            }
            else
            {
                finalReturns.Add(new Return(returns[0][i].Date, weightedSumStrategyValues / sumStrategyValues, sumStrategyValues));
            }
        }
        
        return finalReturns;
    }
}