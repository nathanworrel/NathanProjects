using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.StrategySecondaryProduct;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class StrategySecondaryProductServiceTest: IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }
    
    public StrategySecondaryProductServiceTest(TestDatabaseFixture fixture)
        => Fixture = fixture;
    
    public void Dispose() 
    { 
        Fixture.Reset(); 
    }

    [Fact]
    public void StrategySecondaryProductService_Get()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int id = AddToDb.AddStrategySecondaryProduct(db, _config);
        Assert.NotNull(service.Get(id));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Get_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        Assert.Null(service.Get(1));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Find_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        Assert.Null(service.Find(1));
    }
    
    [Fact]
    public void StrategySecondaryProductService_GetAll_Empty()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        Assert.Empty(service.GetAll());
    }
    
    [Fact]
    public void StrategySecondaryProductService_GetAll()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int id = AddToDb.AddStrategySecondaryProduct(db, _config);
        Assert.Single(service.GetAll());
    }
    
    [Fact]
    public void StrategySecondaryProductService_GetAllByStrategy_Empty()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int id = AddToDb.AddStrategy(db, _config);
        Assert.Empty(service.GetAllByStrategy(id));
    }
    
    [Fact]
    public void StrategySecondaryProductService_GetAllByStrategy_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        Assert.Empty(service.GetAllByStrategy(1));
    }
    
    [Fact]
    public void StrategySecondaryProductService_GetAllByStrategy()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int id = AddToDb.AddStrategy(db, _config);
        int productId = AddToDb.AddStrategySecondaryProduct(db, _config, id);
        
        int id2 = AddToDb.AddStrategy(db, _config);
        int productId2= AddToDb.AddStrategySecondaryProduct(db, _config, id2);
        var list = service.GetAllByStrategy(id);
        Assert.Single(list);
        Assert.Equal(productId, list[0].Id);
        list = service.GetAllByStrategy(id2);
        Assert.Single(list);
        Assert.Equal(productId2, list[0].Id);
    }

    [Fact]
    public void StrategySecondaryProductService_Add()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", strategyId, 1);
        int id = service.Add(post);
        Assert.NotEqual(0, id);
        Assert.NotNull(service.Get(id));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Add_InvalidStrategy()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", 1, 1);
        int id = service.Add(post);
        Assert.Equal(0, id);
        Assert.Null(service.Get(id));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Add_NullProduct()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, null, strategyId, 1);
        int id = service.Add(post);
        Assert.Equal(0, id);
        Assert.Null(service.Get(id));
    }

    [Fact]
    public void StrategySecondaryProductService_Delete()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int id = AddToDb.AddStrategySecondaryProduct(db, _config);
        var secondaryProduct = service.Find(id);
        Assert.NotNull(secondaryProduct);
        Assert.NotNull(service.Delete(secondaryProduct));
    }

    [Fact]
    public void StrategySecondaryProductService_Set_ToEmpty()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", strategyId, 1);
        int runtimeId1 = service.Add(post);
        service.SetRuntimes(strategy, new List<StrategySecondaryProductPost>());
        Assert.Null(service.Find(runtimeId1));
        Assert.Empty(service.GetAllByStrategy(strategyId));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Set_ToAllNew()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", strategyId, 1);
        int runtimeId1 = service.Add(post);
        service.SetRuntimes(strategy, new List<StrategySecondaryProductPost>()
        {
            new StrategySecondaryProductPost(0, "TQQQ", strategyId, 1),
        });
        Assert.Null(service.Find(runtimeId1));
        Assert.Single(service.GetAllByStrategy(strategyId));
    }
    
    [Fact]
    public void StrategySecondaryProductService_Set_ToSomeNew()
    {
        using var db = Fixture.CreateContext();
        StrategySecondaryProductsService service =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        int strategyId = AddToDb.AddStrategy(db, _config);
        var strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategy = strategyService.Find(strategyId);
        Assert.NotNull(strategy);
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", strategyId, 1);
        int runtimeId1 = service.Add(post);
        StrategySecondaryProductPost post2 = new StrategySecondaryProductPost(0, "TQQQ", strategyId, 1);
        int runtimeId2 = service.Add(post2);
        service.SetRuntimes(strategy, new List<StrategySecondaryProductPost>()
        {
            new StrategySecondaryProductPost(runtimeId2, "TQQQ", strategyId, 1),
            new StrategySecondaryProductPost(0, "AAPL", strategyId, 2),

        });
        Assert.Null(service.Find(runtimeId1));
        Assert.Equal(2, service.GetAllByStrategy(strategyId).Count);
    }
}