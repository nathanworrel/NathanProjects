using AutoMapper;
using FishyLibrary.Enums;
using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class TradeServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }
    
    public TradeServiceTest(TestDatabaseFixture fixture)
        => Fixture = fixture;
    
    public void Dispose() 
    { 
        Fixture.Reset(); 
    }

    [Fact]
    public void TradeService_Get()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddTrade(db, _config);
        var trade = service.Get(id);
        Assert.NotNull(trade);
        Assert.NotNull(trade.Strategy);
    }
    
    [Fact]
    public void TradeService_Find()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddTrade(db, _config);
        var trade = service.Find(id);
        Assert.NotNull(trade);
        Assert.NotNull(trade.Strategy);
    }
    
    [Fact]
    public void TradeService_Get_Invalid()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        var trade = service.Get(1);
        Assert.Null(trade);
    }
    
    [Fact]
    public void TradeService_Find_Invalid()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        var trade = service.Find(1);
        Assert.Null(trade);
    }

    [Fact]
    public void TradeService_GetAll_Empty()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public void TradeService_GetAll()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddTrade(db, _config);
        Assert.Single(service.GetAll());
    }
    
    [Fact]
    public void TradeService_Add()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        TradePost post = new TradePost(new DateTime(2024, 12, 14), new DateTime(2024, 12, 15),
            1, 100, 1, 99, 
            (int) TradeStatus.ACCEPTED, 1234, (int ) Side.BUY, strategyId);
        int id = service.Add(post);
        Assert.NotEqual(0, id);
        Assert.Single(service.GetAll());
    }
    
    [Fact]
    public void TradeService_Add_Invalid_Strategy()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        TradePost post = new TradePost(new DateTime(2024, 12, 14), new DateTime(2024, 12, 15),
            1, 100, 1, 99, 
            (int) TradeStatus.ACCEPTED, 1234, (int) Side.BUY, 10);
        int id = service.Add(post);
        Assert.Equal(0, id);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void TradeService_Edit()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        int id = AddToDb.AddTrade(db, _config, strategyId);
        var trade = service.Find(id);
        Assert.NotNull(trade);
        var newTrade = new TradePost(new DateTime(2025, 1, 1), new DateTime(2025, 1, 1),
            5, 50, 3, 49, 
            (int) TradeStatus.AWAITING_CONDITION, 12345678, (int) Side.SELL, strategyId);
        newTrade.Id = id;
        int result = service.Edit(trade, newTrade);
        Assert.NotEqual(0, result);
        trade = service.Find(id);
        Assert.NotNull(trade);
        Assert.Equal(newTrade.Side, trade.Side);
        Assert.Equal(newTrade.StrategyId, trade.StrategyId);
        Assert.Equal(newTrade.TimeModified.ToUniversalTime(), trade.TimeModified);
        Assert.Equal(newTrade.TimePlaced.ToUniversalTime(), trade.TimePlaced);
        Assert.Equal(newTrade.PricePlaced, trade.PricePlaced);
        Assert.Equal(newTrade.PriceFilled, trade.PriceFilled);
        Assert.Equal(newTrade.QuantityFilled, trade.QuantityFilled);
        Assert.Equal(newTrade.QuantityPlaced, trade.QuantityPlaced);
        Assert.Equal(newTrade.Status, trade.Status);
    }
    
    [Fact]
    public void TradeService_Edit_StrategyId()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        int id = AddToDb.AddTrade(db, _config, strategyId);
        var trade = service.Find(id);
        Assert.NotNull(trade);
        var tradePost = _config.CreateMapper().Map<TradePost>(trade);
        tradePost.StrategyId = 100;
        tradePost.QuantityFilled = 1000;
        int result = service.Edit(trade, tradePost);
        Assert.Equal(0, result);
        trade = service.Find(id);
        Assert.NotNull(trade);
        Assert.Equal(strategyId, trade.StrategyId);
        Assert.NotEqual(1000, trade.QuantityFilled);
    }

    [Fact]
    public void TradeService_Delete()
    {
        using var db = Fixture.CreateContext();
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddTrade(db, _config);
        var trade = service.Find(id);
        Assert.NotNull(trade);
        var tradePost = service.Delete(trade);
        Assert.NotNull(tradePost);
        Assert.Empty(service.GetAll());
    }
}