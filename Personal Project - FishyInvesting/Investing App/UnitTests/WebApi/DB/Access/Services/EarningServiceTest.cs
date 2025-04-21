using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using FishyLibrary.Utils;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class EarningServiceTest
{
    private static readonly string Product = "QQQ";
    private static readonly DateTime StartDate = new DateTime(2020, 1, 1);
    private static readonly Decimal StartPrice = 100;

    private static readonly List<StockData> StockData = new List<StockData>()
    {
        new StockData(Product, StartDate, StartPrice),
        new StockData(Product, StartDate.AddDays(1), StartPrice + 5), // up 5%
        new StockData(Product, StartDate.AddDays(2), StartPrice + 10), // up 4.7%s
        new StockData(Product, StartDate.AddDays(3), StartPrice + 15), // down 9.523%
        new StockData(Product, StartDate.AddDays(4), StartPrice + 20), // up 5.263%
    };
    
    [Fact]
    public void EarningService_GenerateReturns_NotInMarket()
    {
        // Arrange
        EarningService service = new EarningService();
        List<Trade> trades = new List<Trade>();
        
        // Act
        var returns = service.GenerateReturns(StockData, trades);
        
        // Assert
        Assert.NotEmpty(returns);
        Assert.Equal(StockData.Count, returns.Count);
        Assert.Equal((StartDate), returns[0].Date);
        Assert.Equal(StartDate.AddDays(1), returns[1].Date);
        Assert.Equal(StartDate.AddDays(2), returns[2].Date);
        Assert.Equal(StartDate.AddDays(3), returns[3].Date);
        Assert.Equal(StartDate.AddDays(4), returns[4].Date);
        Assert.Equal(1, returns[0].Returns);
        Assert.Equal(1, returns[1].Returns);
        Assert.Equal(1, returns[2].Returns);
        Assert.Equal(1, returns[3].Returns);
        Assert.Equal(1, returns[4].Returns);
    }
    
    [Fact]
    public void EarningService_GenerateReturns_Buy_TimeEqualsLowerBound()
    {
        // Arrange
        EarningService service = new EarningService();
        List<Trade> trades = new List<Trade>()
        {
            new Trade(StartDate, 5, StartPrice, (int)Side.BUY, 1),
        };
        
        // Act
        var returns = service.GenerateReturns(StockData, trades);
        
        // Assert
        Assert.Equal(1, (double) returns[0].Returns, 0.01);
        Assert.Equal(1.05, (double) returns[1].Returns, 0.01);
    }
    
    [Fact]
    public void EarningService_GenerateReturns_TimeEqualsUpperBound()
    {
        // Arrange
        EarningService service = new EarningService();
        List<Trade> trades = new List<Trade>()
        {
            new Trade(StartDate.AddDays(1), 5, StartPrice + 5, (int)Side.BUY, 1),
        };
        
        // Act
        var returns = service.GenerateReturns(StockData, trades);
        
        // Assert
        Assert.Equal(StockData.Count, returns.Count);
        Assert.Equal(1,(double) returns[0].Returns, 0.01);
        Assert.Equal(1,(double) returns[1].Returns, 0.01);
        Assert.Equal(1.047, (double) returns[2].Returns, 0.01);
    }
    
    [Fact]
    public void EarningService_GenerateReturns_InMarket_TimeBelowLowerBound()
    {
        // Arrange
        EarningService service = new EarningService();
        List<Trade> trades = new List<Trade>()
        {
            new Trade(StartDate.Subtract(new TimeSpan(3, 0,0,0)), 5, StartPrice, (int)Side.BUY, 1),
            new Trade(StartDate.Subtract(new TimeSpan(2, 0,0,0)), 5, StartPrice + 5, (int)Side.SELL, 1),
            new Trade(StartDate.Subtract(new TimeSpan(1, 0,0,0)), 5, StartPrice - 5, (int)Side.BUY, 1)
        };
        
        // Act
        var returns = service.GenerateReturns(StockData, trades);
        
        // Assert
        Assert.Equal(StockData.Count, returns.Count);
        Assert.Equal((double) (StartPrice / (StartPrice - 5)) ,(double) returns[0].Returns, 0.01);
    }
}