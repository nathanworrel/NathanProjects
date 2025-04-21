using FishyLibrary.Models.Trade;
using FishyLibrary.Models;
using DateTime = System.DateTime;
using Position = FishyLibrary.Helpers.Position;

namespace UnitTests.FishyLibrary.Helpers;

public class PositionTest
{
    private static readonly decimal StartingPrice = 10;
    private static readonly DateTime StartTime = new DateTime(2024, 12, 1);
    
    /*
     * returns a position currently 100% in the market in a fresh interval
     */
    private Position AllBoughtPosition()
    {
        Position position = new Position();
        Trade trade = new Trade(StartTime, 5, StartingPrice, (int)Side.BUY, 1);
        position.AddTradeInInterval(trade);
        position.IncreaseInterval(new StockData("", StartTime, StartingPrice));
        return position;
    }
    
    /*
     * returns a position currently 50% in the market in a fresh interval
     */
    private Position HalfBoughtPosition()
    {
        Position position = new Position();
        Trade trade = new Trade(StartTime, 5, StartingPrice, (int)Side.BUY, 0.5m);
        position.AddTradeInInterval(trade);
        position.IncreaseInterval(new StockData("", StartTime, StartingPrice));
        return position;
    }

    private decimal GetPortionMultiplier(decimal multiplier, decimal portion = 0.5m)
    {
        return ((multiplier - 1) * portion) + 1;
    }

    [Fact]
    public void GetHalfMultiplierTest()
    {
        Assert.Equal(1.1m, GetPortionMultiplier(1.2m));
        Assert.Equal(1m, GetPortionMultiplier(1m));
        Assert.Equal(0.9m, GetPortionMultiplier(0.8m));
    }
    
    [Fact]
    public void Position_Add_Buy()
    {
        // Arrange
        Position position = new Position();
        Trade trade = new Trade(StartTime, 5, 20, (int)Side.BUY, 0.5m);
        
        // Act
        position.AddTradeInInterval(trade);
        
        // Assert
        Assert.Equal(trade.DesiredAllocation, position.CurrentAllocation);
    }
    
    [Fact]
    public void Position_Add_Sell()
    {
        // Arrange
        Position position = new Position();
        Trade trade = new Trade(StartTime, 5, 20, (int)Side.SELL, 0);
        
        // Act
        position.AddTradeInInterval(trade);
        
        // Assert
        Assert.Equal(trade.DesiredAllocation, position.CurrentAllocation);
    }

    [Fact]
    public void Position_Returns_All_BuyInterval()
    {
        // Arrange
        decimal multiplier = 1.1m;
        Position position = new Position();
        Trade trade = new Trade(StartTime, 5, StartingPrice, (int)Side.BUY, 1m);
        
        // Act
        position.AddTradeInInterval(trade);
        Return returns = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multiplier));
        
        // Assert
        Assert.Equal(multiplier, returns.Returns);
    }
    
    [Fact]
    public void Position_Returns_All_Hold_PriceChange()
    {
        // Arrange
        var multOne = 1.1m;
        Position position = AllBoughtPosition();
        
        // Act
        Return returns = 
            position.IncreaseInterval(new StockData( "", StartTime.AddDays(2), StartingPrice * multOne));
        
        // Assert
        Assert.Equal(multOne, returns.Returns);
    }
    
    [Fact]
    public void Position_Returns_All_Hold_NoPriceChange()
    {
        // Arrange
        var multOne = 1.1m;
        Position position = AllBoughtPosition();
        
        // Act
        position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Return returns =
            position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * multOne));
        
        // Assert
        Assert.Equal(1m , returns.Returns);
    }

    [Fact]
    public void Position_Returns_Half_BuyInterval()
    {
        // Arrange
        decimal multiplier = 1.1m;
        Position position = new Position();
        Trade trade = new Trade(
            StartTime, 5, StartingPrice, (int)Side.BUY, 0.5m);
        
        // Act
        position.AddTradeInInterval(trade);
        Return returns = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multiplier));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multiplier), returns.Returns);
    }
    
    [Fact]
    public void Position_Tenth_BuyInterval()
    {
        // Arrange
        decimal multiplier = 1.1m;
        var allocation = 0.1m;
        Position position = new Position();
        Trade trade = new Trade(
            StartTime, 5, StartingPrice, (int)Side.BUY, allocation);
        
        // Act
        position.AddTradeInInterval(trade);
        Return returns = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multiplier));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multiplier, allocation), returns.Returns);
    }
    
    [Fact]
    public void Position_Half_Hold_PriceChange()
    {
        // Arrange
        var multOne = 1.1m;
        var multTwo = 1.2m;
        Position position = HalfBoughtPosition();
        
        // Act
        position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Return returnsDayTwo = position.IncreaseInterval(new StockData( "", StartTime.AddDays(2), StartingPrice * multOne * multTwo));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multTwo), returnsDayTwo.Returns);
    }
    
    [Fact]
    public void Position_Half_Hold_NoPriceChange()
    {
        // Arrange
        var multOne = 1.1m;
        Position position = HalfBoughtPosition();
        
        // Act
        position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Return returnsDayTwo = position.IncreaseInterval(new StockData( "", StartTime.AddDays(2), StartingPrice * multOne));
        
        // Assert
        Assert.Equal(1m, returnsDayTwo.Returns);
    }
    
    [Fact]
    public void Position_All_Sell_Interval()
    {
        // Arrange
        Position position = AllBoughtPosition();
        var multOne = 1.1m;
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * multOne, (int)Side.SELL, 0);
        
        // Act
        position.AddTradeInInterval(trade);
        Return returnsThree = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * 0.9m));
        
        // Assert
        Assert.Equal(multOne, returnsThree.Returns);
    }
    
    [Fact]
    public void Position_Half_Sell_Interval()
    {
        // Arrange
        Position position = HalfBoughtPosition();
        var multOne = 1.1m;
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * multOne, (int)Side.SELL, 0);
        
        // Act
        position.AddTradeInInterval(trade);
        Return returnsThree = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * 0.9m));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multOne), returnsThree.Returns);
    }
    
    [Fact]
    public void Position_All_Sell_OverIntervals_Gain()
    {
        // Arrange
        Position position = AllBoughtPosition();
        var multOne = 1.25m;
        var earning = 1.1m;
        
        // Act
        Return returnsOne = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * earning, (int)Side.SELL, 0);
        position.AddTradeInInterval(trade);
        Return returnsThree = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * 0.9m));
        
        // Assert
        Assert.Equal(multOne, returnsOne.Returns);
        Assert.Equal((StartingPrice * earning) / (StartingPrice * multOne), returnsThree.Returns);
        Assert.Equal(earning, returnsOne.Returns * returnsThree.Returns);
    }
    
    [Fact]
    public void Position_Half_Sell_OverIntervals_Gain()
    {
        // Arrange
        Position position = HalfBoughtPosition();
        var multOne = 1.25m;
        var earning = 1.1m;
        
        // Act
        Return returnsOne = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * earning, (int)Side.SELL, 0);
        position.AddTradeInInterval(trade);
        Return returnsThree = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * 0.9m));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multOne), returnsOne.Returns);
        Assert.Equal(GetPortionMultiplier(StartingPrice * earning / (StartingPrice * multOne)), returnsThree.Returns);
        Assert.Equal((double)GetPortionMultiplier(earning), (double)(returnsOne.Returns * returnsThree.Returns), 0.01);
    }
    
    [Fact]
    public void Position_All_Sell_OverIntervals_Loss()
    {
        // Arrange
        Position position = AllBoughtPosition();
        var multOne = 0.95m;
        var earning = 0.9m;
        
        // Act
        Return returnsOne = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * earning, (int)Side.SELL, 0);
        position.AddTradeInInterval(trade);
        Return returnsThree = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * multOne));
        
        // Assert
        Assert.Equal(multOne, returnsOne.Returns);
        Assert.Equal(StartingPrice * earning / (StartingPrice * multOne), returnsThree.Returns);
        Assert.Equal(earning, returnsOne.Returns * returnsThree.Returns);
    }
    
    [Fact]
    public void Position_Half_Sell_OverIntervals_Loss()
    {
        // Arrange
        Position position = HalfBoughtPosition();
        var multOne = 1.25m;
        var earning = 0.9m;
        Trade trade = new Trade(
            StartTime.AddDays(2).AddMinutes(1), 5, StartingPrice * 0.9m, (int)Side.SELL, 0);
        
        // Act
        Return returnsOne = position.IncreaseInterval(new StockData( "", StartTime.AddDays(1), StartingPrice * multOne));
        position.AddTradeInInterval(trade);
        Return returnsTwo = position.IncreaseInterval(new StockData("", StartTime.AddDays(3), StartingPrice * 0.9m));
        
        // Assert
        Assert.Equal(GetPortionMultiplier(multOne), returnsOne.Returns);
        Assert.Equal(GetPortionMultiplier(earning / multOne), returnsTwo.Returns);
        Assert.Equal((double) GetPortionMultiplier(earning), (double)(returnsOne.Returns * returnsTwo.Returns), 0.02);
    }

    [Fact]
    public void Position_All_BuyAndSell_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(multiplier, returns.Returns);
    }
    
    [Fact]
    public void Position_Quarter_BuyAndSell_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(GetPortionMultiplier(multiplier, portion), returns.Returns);
    }
    
    [Fact]
    public void Position_All_BuyAndSell_Interval_Loss()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 0.75m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(multiplier, returns.Returns);
    }
    
    [Fact]
    public void Position_Quarter_BuyAndSell_Interval_Loss()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 0.75m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(GetPortionMultiplier(multiplier, portion), returns.Returns);
    }
    
    [Fact]
    public void Position_AddAll_All_BuyAndSell_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, sellTrade});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(multiplier, returns.Returns);
    }
    
    [Fact]
    public void Position_AddAll_Quarter_BuyAndSell_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);

        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, sellTrade});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * 1.2m));

        // Assert
        Assert.Equal(GetPortionMultiplier(multiplier, portion), returns.Returns);
    }
    
    [Fact]
    public void Position_All_BuySellBuy_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var multiplier2 = 1.2m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, 1);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        position.AddTradeInInterval(buyTrade2);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal(multiplier * multiplier2, returns.Returns);
    }
    
    [Fact]
    public void Position_Quarter_BuySellBuy_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var multiplier2 = 1.2m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, portion);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        position.AddTradeInInterval(buyTrade2);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal((double)GetPortionMultiplier(multiplier * multiplier2, portion), (double)returns.Returns, 0.02);
    }
    
    [Fact]
    public void Position_All_BuySellBuy_Interval_Loss()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 0.75m;
        var multiplier2 = 0.9m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, 1);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        position.AddTradeInInterval(buyTrade2);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal(multiplier * multiplier2, returns.Returns);
    }
    
    [Fact]
    public void Position_Quarter_BuySellBuy_Interval_Loss()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 0.75m;
        var multiplier2 = 0.9m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, portion);

        // Act
        position.AddTradeInInterval(buyTrade);
        position.AddTradeInInterval(sellTrade);
        position.AddTradeInInterval(buyTrade2);
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal((double)(GetPortionMultiplier(multiplier, portion) * GetPortionMultiplier(multiplier2, portion)), (double)returns.Returns, 0.01);
    }
    
    [Fact]
    public void Position_AddAll_All_BuySellBuy_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var multiplier2 = 1.2m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, 1);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, 1);

        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, sellTrade, buyTrade2});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal(multiplier * multiplier2, returns.Returns);
    }
    
    [Fact]
    public void Position_AddAll_Quarter_BuySellBuy_Interval_Gain()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, StartingPrice));
        var multiplier = 1.5m;
        var multiplier2 = 1.2m;
        var portion = 0.25m;
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 5, StartingPrice, (int) Side.BUY, portion);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 5, StartingPrice * multiplier, (int) Side.SELL, 0);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 5, StartingPrice, (int) Side.BUY, portion);

        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, sellTrade, buyTrade2});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice * multiplier2));

        // Assert
        Assert.Equal((double)GetPortionMultiplier(multiplier * multiplier2, portion),(double) returns.Returns, 0.02);
    }
    
    [Fact]
    public void Position_AddAll_MultipleSellPrices()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 2, 10, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 1, 11, (int) Side.SELL, 0.25m);
        Trade sellTrade2 = new Trade(StartTime.AddMinutes(10), 1, 12, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, sellTrade, sellTrade2});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice));
        
        // Assert
        /*
         * Amount Allocated = $40
         * Sell 1 Profit = $1
         * Sell 2 Profit = $2
         * Amount Allocated = $43
         * Returns = 1.075%
         */
        Assert.Equal((double)1.075m, (double)returns.Returns, 0.005);
    }
    
    [Fact]
    public void Position_AddAll_MultipleBuyPrices()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 1, 10, (int) Side.BUY, 0.25m);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(5), 1, 12, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 2, 14, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, buyTrade2, sellTrade});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice));

        // Assert
        /*
         * Amount Allocated = $40
         * Buy 1 Profit = $4 (1.4 returns)
         * Buy 2 Profit = $2 (1.166 returns)
         * Amount Allocated = $46
         * Returns = 1.15 (1.4 * 0.25 + 1.166 * 0.25 * 1 * 0.5))
         */
        Assert.Equal((double)1.15m, (double)returns.Returns, 0.01);
    }
    
    [Fact]
    public void Position_AddAll_MultipleBuyAndSellPrices()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 1, 10, (int) Side.BUY, 0.25m);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(5), 1, 12, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(10), 1, 14, (int) Side.SELL, 0.25m);
        Trade sellTrade2 = new Trade(StartTime.AddMinutes(10), 1, 16, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, buyTrade2, sellTrade, sellTrade2});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), StartingPrice));

        // Assert
        /*
         * Amount Allocated = $40
         * Buy 1 Profit = $4 | $6
         * Buy 2 Profit = $4 | $2
         * Amount Allocated = $48
         * Returns = 1.2%
         */
        
        /*
         * Amount Allocated = $40
         * Average Buy = $11
         * Average Sell = $15
         * Amount Allocated = ($15 - $11) * 2 stocks + $40 = $48
         * Returns = 1.2%
         */
        
        /*
         * Amount Allocated = $40
         * Average Buy = $11
         * Sell 1 Profit = $3
         * Sell 2 Profit = $5
         * Amount Allocated = $48
         * Returns = 1.2%
         */
        Assert.Equal((double) 1.2m, (double)returns.Returns, 0.01);
    }
    
    [Fact]
    public void Position_AddAll_MultipleBuyPrices_SellOverIntervals()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 1, 10, (int) Side.BUY, 0.25m);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(5), 1, 12, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(20), 2, 14, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade, buyTrade2});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), 13));
        position.AddTradeInInterval(sellTrade);
        Return returns2 = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(25), 13));
        // Assert
        /*
         * Amount Allocated = $40
         * Buy 1 Profit = $3
         * Buy 2 Profit = $1
         * Amount Allocated = $44
         * Returns Interval 1 = 1.1%
         * 
         * Buy 1 Profit = $1
         * Buy 2 Profit = $1
         * Amount Allocated = $46
         * Returns Interval 1 = 1.045%
         */
        Assert.Equal((double)1.1m, (double) returns.Returns, 0.01);
        Assert.Equal((double)1.045m, (double) returns2.Returns, 0.01);
        Assert.Equal((double)1.15m, (double)(returns.Returns * returns2.Returns), 0.02);
    }
    
    [Fact]
    public void Position_AddAll_MultipleSellPrices_BuyPreviousInterval()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 2, 10, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(15), 1, 11, (int) Side.SELL, 0.25m);
        Trade sellTrade2 = new Trade(StartTime.AddMinutes(15), 1, 12, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(10), 10.5m));
        position.AddAllTradesInInterval(new List<Trade> {sellTrade, sellTrade2});
        Return returns2 = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(25), 13));

        // Assert
        /*
         * Amount Allocated = $40
         * Sell 1 Profit = $1
         * Sell 2 Profit = $2
         * Amount Allocated = $43
         * Returns = 1.075%
         */
        Assert.Equal(1.025m, returns.Returns);
        Assert.Equal((double)1.04878m,(double) returns2.Returns, 0.01);
        Assert.Equal((double)1.075m, (double)(returns.Returns * returns2.Returns), 0.01);
    }
    
    [Fact]
    public void Position_AddAll_MultipleBuyPrices_BuyOverIntervals()
    {
        // Arrange
        Position position = new Position(new StockData("", StartTime, 10));
        Trade buyTrade = new Trade(StartTime.AddMinutes(5), 1, 10, (int) Side.BUY, 0.25m);
        Trade buyTrade2 = new Trade(StartTime.AddMinutes(15), 1, 12, (int) Side.BUY, 0.5m);
        Trade sellTrade = new Trade(StartTime.AddMinutes(20), 2, 14, (int) Side.SELL, 0);
        
        // Act
        position.AddAllTradesInInterval(new List<Trade> {buyTrade});
        Return returns = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(15), 11));
        position.AddTradeInInterval(buyTrade2);
        Return returns2 = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(25), 13));
        position.AddTradeInInterval(sellTrade);
        Return returns3 = position.IncreaseInterval(new StockData("", StartTime.AddMinutes(25), 15));
        // Assert
        /*
         * Amount Allocated = $40
         * Buy 1 Profit = $1
         * Amount Allocated = $41
         * Returns Interval 1 = 1.025
         *
         * Buy 1 Profit = $2
         * Buy 2 Profit = $1
         * Amount Allocated = $44
         * Returns Interval 2 = 1.073%
         *
         * Buy 1 Profit = $1
         * Buy 2 Profit = $1
         * Amount Allocated = $46
         * Returns Interval 3 = 1.045%
         */
        Assert.Equal(1.025m, returns.Returns);
        Assert.Equal((double)1.045m, (double)returns3.Returns, 0.01);
        Assert.Equal((double)1.15m, (double)(returns.Returns * returns2.Returns * returns3.Returns), 0.015);
    }
}