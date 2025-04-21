using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Strategy;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class StrategyServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }
    
    public StrategyServiceTest(TestDatabaseFixture fixture)
        => Fixture = fixture;
    
    public void Dispose() 
    { 
        Fixture.Reset(); 
    }

    [Fact]
    public void StrategyService_AddStrategy()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", "QQQ", 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.NotEqual(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_InvalidParameter()
    {
        using var db = Fixture.CreateContext();
        int paramId = 1;
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", "QQQ", 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_InvalidAccountId()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = 100;
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", "QQQ", 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_NullName()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, null, "test description", "QQQ", 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_NullDescription()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", null, "QQQ", 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_NullProduct()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", null, 
            false, "", 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_NullIntermediateData()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", "QQQ", 
            false, null, 100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.NotEqual(0, id);
    }
    
    [Fact]
    public void StrategyService_AddStrategy_NegativeAmountAllocated()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategy = new StrategyPost(0, "test", "test description", "QQQ", 
            false, null, -100, false, 0, paramId, 
            DateTime.Now, accountId);
        int id = service.AddStrategy(strategy);
        Assert.Equal(0, id);
    }

    [Fact]
    public void StrategyService_Get_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);

        var strategy = service.Get(1);
        Assert.Null(strategy);
    }
    
    [Fact]
    public void StrategyService_Find_Invalid()
    {
        using var db = Fixture.CreateContext();
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);

        var strategy = service.Find(1);
        Assert.Null(strategy);
    }
    
    [Fact]
    public void StrategyService_Get()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategy = service.Get(id);
        Assert.NotNull(strategy);
        Assert.NotNull(strategy.Parameters);
        Assert.NotNull(strategy.Account);
        Assert.NotEqual(0, strategy.Parameters.Id);
        Assert.NotEqual(0, strategy.Account.Id);
    }
    
    [Fact]
    public void StrategyService_Find()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategy = service.Find(id);
        Assert.NotNull(strategy);
        Assert.NotNull(strategy.Parameters);
        Assert.NotNull(strategy.Account);
        Assert.NotEqual(0, strategy.Parameters.Id);
        Assert.NotEqual(0, strategy.Account.Id);
    }

    [Fact]
    public void StrategyService_GetAll_Empty()
    {
        using var db = Fixture.CreateContext();
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var list = service.GetAll();
        Assert.Empty(list);
    }

    [Fact]
    public void StrategyService_GetAll()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var list = service.GetAll();
        Assert.NotEmpty(list);
        var strategy = list[0];
        Assert.NotNull(strategy.Parameters);
        Assert.NotNull(strategy.Account);
        Assert.NotEqual(0, strategy.Parameters.Id);
        Assert.NotEqual(0, strategy.Account.Id);
    }

    [Fact]
    public void StrategyService_GetAllForAccount()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int accountId = AddToDb.AddAccount(db, _config);
        int id = AddToDb.AddStrategy(db, _config, paramId, accountId);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var list = service.GetAllForAccount(id);
        Assert.NotEmpty(list);
        Assert.Equal(id, list[0].Id);
        var strategy = list[0];
        Assert.NotNull(strategy.Parameters);
        Assert.NotNull(strategy.Account);
        Assert.NotEqual(0, strategy.Parameters.Id);
        Assert.NotEqual(0, strategy.Account.Id);
    }
    
    
    [Fact]
    public void StrategyService_GetAllForAccount_Empty()
    {
        using var db = Fixture.CreateContext();
        int accountId = AddToDb.AddAccount(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var list = service.GetAllForAccount(accountId);
        Assert.Empty(list);
    }
    
    [Fact]
    public void StrategyService_GetAllForAccount_InvalidAccountId()
    {
        using var db = Fixture.CreateContext();
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var list = service.GetAllForAccount(100);
        Assert.Empty(list);
    }

    [Fact]
    public void StrategyService_EditStrategy_Name()
    {
        var newName = "newName";
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.Name = newName;
        service.EditStrategy(strategy, strategyPost);
        Assert.Equal(newName, strategyPost.Name);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_Description()
    {
        var newName = "newName";
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.Description = newName;
        service.EditStrategy(strategy, strategyPost);
        Assert.Equal(newName, strategyPost.Description);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_Product()
    {
        var newName = "AAPL";
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.Product = newName;
        service.EditStrategy(strategy, strategyPost);
        Assert.Equal(newName, strategyPost.Product);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_Product_AfterTrade()
    {
        var newName = "AAPL";
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        AddToDb.AddTrade(db, _config, id);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.Product = newName;
        int returnId = service.EditStrategy(strategy, strategyPost);
        Assert.Equal(0, returnId);
        strategy = service.Find(id);
        Assert.NotEqual(newName, strategy.Product);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_Parameters()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        int paramId = AddToDb.AddParameters(db, _config, 2);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.ParameterId = paramId;
        service.EditStrategy(strategy, strategyPost);
        Assert.Equal(paramId, strategyPost.ParameterId);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_Parameters_AfterTrade()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        int paramId = AddToDb.AddParameters(db, _config, 2);
        int tradeId = AddToDb.AddTrade(db, _config, id);
        Assert.NotEqual(0 , tradeId);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.Id = strategy.Id;
        strategyPost.ParameterId = paramId;
        int returnId = service.EditStrategy(strategy, strategyPost);
        Assert.Equal(0, returnId);
        strategy = service.Find(id);
        Assert.NotEqual(paramId, strategy.ParameterId);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_AmountAllocated()
    {
        var newName = 1001;
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.AmountAllocated = newName;
        service.EditStrategy(strategy, strategyPost);
        Assert.Equal(newName, strategyPost.AmountAllocated);
    }
    
    [Fact]
    public void StrategyService_EditStrategy_AmountAllocated_Negative()
    {
        var newName = -1001;
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? strategy = service.Find(id);
        Assert.NotNull(strategy);
        StrategyPost strategyPost = _config.CreateMapper().Map<StrategyPost>(strategy);
        strategyPost.AmountAllocated = newName;
        int returnId = service.EditStrategy(strategy, strategyPost);
        Assert.Equal(0, returnId);
        strategy = service.Find(id);
        Assert.NotEqual(newName, strategy.AmountAllocated);
    }

    [Fact]
    public void StrategyService_DeleteStrategy()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategyEntity = service.Find(id);
        Assert.NotNull(strategyEntity);
        var strategy = service.Delete(strategyEntity);
        Assert.NotNull(strategy);
    }
    
    [Fact]
    public void StrategyService_DeleteStrategy_WithForiegnKeys()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddStrategy(db, _config);
        int runtimeId = AddToDb.AddStrategyRuntime(db, _config, id);
        int secondaryProduct = AddToDb.AddStrategySecondaryProduct(db, _config, id);
        StrategyService service = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        var strategyEntity = service.Find(id);
        Assert.NotNull(strategyEntity);
        var strategy = service.Delete(strategyEntity);
        Assert.NotNull(strategy);
        // Ensure runtime is deleted
        StrategyRuntimeService runtimeService = new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, _config.CreateMapper());
        var runtime = runtimeService.Get(runtimeId);
        Assert.Null(runtime);
        // Ensure secondary product is deleted
        StrategySecondaryProductsService productsService =
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db,
                _config.CreateMapper());
        var product = productsService.Get(secondaryProduct);
        Assert.Null(product);
    }
}