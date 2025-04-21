using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.StrategyRuntime;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class StrategyRuntimeServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }
    
    public StrategyRuntimeServiceTest(TestDatabaseFixture fixture)
        => Fixture = fixture;
    
    public void Dispose() 
    { 
        Fixture.Reset(); 
    }

    [Fact]
    public void StrategyRuntimeService_Get()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddStrategyRuntime(db, _config);
        var result = service.Get(id);
        Assert.NotNull(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_Get_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.Get(100);
        Assert.Null(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_Find()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddStrategyRuntime(db, _config);
        var result = service.Find(id);
        Assert.NotNull(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_Find_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.Find(100);
        Assert.Null(result);
    }

    [Fact]
    public void StrategyRuntimeService_GetAll_Empty()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.GetAll();
        Assert.Empty(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_GetAll()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int id = AddToDb.AddStrategyRuntime(db, _config);
        var result = service.GetAll();
        Assert.Single(result);
    }

    [Fact]
    public void StrategyRuntimeService_GetAllByStrategy_Empty()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.GetAllByStrategy(id);
        Assert.Empty(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_GetAllByStrategy_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.GetAllByStrategy(100);
        Assert.Empty(result);
    }
    
    [Fact]
    public void StrategyRuntimeService_GetAllByStrategy()
    {
        using var db = Fixture.CreateContext();
        int strategyId1= AddToDb.AddStrategy(db, _config);
        int runtimeId1 = AddToDb.AddStrategyRuntime(db, _config, strategyId1);
        Assert.NotEqual(0 , runtimeId1);
        
        int strategyId2= AddToDb.AddStrategy(db, _config);
        int runtimeId2 = AddToDb.AddStrategyRuntime(db, _config, strategyId2);
        Assert.NotEqual(0 , runtimeId2);
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var result = service.GetAllByStrategy(strategyId1);
        Assert.Single(result);
        Assert.Equal(strategyId1, result.First().Id);
        
        result = service.GetAllByStrategy(strategyId2);
        Assert.Single(result);
        Assert.Equal(strategyId2, result.First().Id);
    }

    [Fact]
    public void StrategyRuntimeService_Add()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        int id = service.Add(strategyId, "10:00:00");
        Assert.NotEqual(0, id);
        Assert.NotNull(service.Get(strategyId));
    }
    
    [Fact]
    public void StrategyRuntimeService_InvalidStrategy()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int id = service.Add(1, "10:00:00");
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyRuntimeService_InvalidRuntime()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        int id = service.Add(1, "10:00");
        Assert.NotEqual(0, id);
        id = service.Add(1, "10 PM");
        Assert.Equal(0, id);
        id = service.Add(1, "2024/12/14 10:00");
        Assert.Equal(0, id);
    }

    [Fact]
    public void StrategyRuntimeService_Delete()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        int runtimeId = AddToDb.AddStrategyRuntime(db, _config);
        var runtime = service.Find(runtimeId);
        Assert.NotNull(runtime);
        Assert.NotNull(service.Delete(runtime));
    }

    [Fact]
    public void StrategyRuntimeService_Set_ToEmpty()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        StrategyService strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        int runtimeId1 = service.Add(strategyId, "10:00:00");
        service.SetRuntimes(strategy, new List<StrategyRuntimeString>());
        Assert.Null(service.Find(runtimeId1));
        Assert.Empty(service.GetAllByStrategy(strategyId));
    }
    
    [Fact]
    public void StrategyRuntimeService_Set_ToAllNew()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        StrategyService strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        int runtimeId1 = service.Add(strategyId, "10:00:00");
        int runtimeId2 = service.Add(strategyId, "12:00:00");
        // Note: the id matters!
        service.SetRuntimes(strategy, new List<StrategyRuntimeString>()
        {
            new StrategyRuntimeString(0, "11:00:00"),
            new StrategyRuntimeString(0, "12:00:00")
        });
        Assert.Null(service.Find(runtimeId1));
        Assert.Null(service.Find(runtimeId2));
        var runtimes = service.GetAllByStrategy(strategyId);
        Assert.Equal(2, runtimes.Count());
    }
    
    [Fact]
    public void StrategyRuntimeService_Set_ToSomeNew()
    {
        using var db = Fixture.CreateContext();
        StrategyRuntimeService service = 
            new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        StrategyService strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        int runtimeId1 = service.Add(strategyId, "10:00:00");
        int runtimeId2 = service.Add(strategyId, "11:00:00");
        service.SetRuntimes(strategy, new List<StrategyRuntimeString>()
        {
            new StrategyRuntimeString(runtimeId2, "11:00:00"),
            new StrategyRuntimeString(0, "12:00:00"),
            new StrategyRuntimeString(0, "13:00:00")
        });
        Assert.Null(service.Find(runtimeId1));
        Assert.NotNull(service.Find(runtimeId2));
        var runtimes = service.GetAllByStrategy(strategyId);
        Assert.Equal(3, runtimes.Count());
    }
}