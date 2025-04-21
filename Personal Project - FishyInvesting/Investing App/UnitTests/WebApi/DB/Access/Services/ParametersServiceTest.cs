using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyType;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

public class ParametersServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }
    
    public ParametersServiceTest(TestDatabaseFixture fixture)
        => Fixture = fixture;
    
    public void Dispose() 
    { 
        Fixture.Reset(); 
    }

    [Fact]
    public void Parameters_GetByIdWithStrategyType_None()
    {
        using var db = Fixture.CreateContext();
        ParametersService service = 
            new ParametersService(db, _config.CreateMapper() ,NullLogger<ParametersService>.Instance);
        ParametersGet? parameters = service.GetByIdWithStrategyType(1);
        Assert.Null(parameters);
    }

    [Fact]
    public void Parameters_GetByIdWithStrategyType_Single()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        ParametersGet? parametersGet = service.GetByIdWithStrategyType(id);
        Assert.NotNull(parametersGet);
        Assert.Equal(1, parametersGet.StrategyType.Id);
    }
    
    [Fact]
    public void Parameters_GetById_Single()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        Parameters? parameters = service.GetById(id);
        Assert.NotNull(parameters);
    }
    
    [Fact]
    public void Parameters_AddParameter_Invalid_StrategyType()
    {
        using var db = Fixture.CreateContext();
        ParametersService service = 
            new ParametersService(db, _config.CreateMapper() ,NullLogger<ParametersService>.Instance);
        ParametersPost parameters = new ParametersPost(0, new Dictionary<string, double?>(), 1);
        int id = service.AddParameters(parameters);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void Parameters_AddParameter_Null_Params()
    {
        using var db = Fixture.CreateContext();
        int strategyType = AddToDb.AddStrategyType(db, _config);
        ParametersService service = 
            new ParametersService(db, _config.CreateMapper() ,NullLogger<ParametersService>.Instance);
        ParametersPost parameters = new ParametersPost(0, null, strategyType);
        int id = service.AddParameters(parameters);
        Assert.Equal(0, id);
    }

    [Fact]
    public void Parameters_AddParameter()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        Assert.NotEqual(0, id);
    }

    [Fact]
    public void Parameters_GetAll_Empty()
    {
        using var db = Fixture.CreateContext();
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        List<ParametersGet> parametersList = service.GetAll();
        Assert.NotNull(parametersList);
        Assert.Empty(parametersList);
    }
    
    [Fact]
    public void Parameters_GetAll()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        List<ParametersGet> parametersList = service.GetAll();
        Assert.NotNull(parametersList);
        Assert.Single(parametersList);
    }

    [Fact]
    public void Parameters_GetByStrategyType_None()
    {
        using var db = Fixture.CreateContext();
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        List<ParametersGet> parametersGet = service.GetByStrategyType(1);
        Assert.Empty(parametersGet);
    }
    
    [Fact]
    public void Parameters_GetByStrategyType()
    {
        using var db = Fixture.CreateContext();
        int id1 = AddToDb.AddParameters(db, _config, 1);
        int id2 = AddToDb.AddParameters(db, _config, 2);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        List<ParametersGet> parametersGet = service.GetByStrategyType(1);
        Assert.Single(parametersGet);
        Assert.Equal(id1, parametersGet[0].Id);
        parametersGet = service.GetByStrategyType(2);
        Assert.Single(parametersGet);
        Assert.Equal(id2, parametersGet[0].Id);
    }

    [Fact]
    public void Parameters_DeleteByParameters()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        Parameters? parameters = service.GetById(id);
        Assert.NotNull(parameters);
        ParametersPost result = service.DeleteByParameters(parameters);
        Assert.Equal(id, result.Id);
        List<ParametersGet> parametersList = service.GetAll();
        Assert.Empty(parametersList);
    }
    
    [Fact]
    public void Parameters_DeleteByParameters_WithStrategy()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        int strategyId = AddToDb.AddStrategy(db, _config, id);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        Parameters? parameters = service.GetById(id);
        Assert.NotNull(parameters);
        ParametersPost? result = service.DeleteByParameters(parameters);
        Assert.Null(result);
        List<ParametersGet> parametersList = service.GetAll();
        Assert.NotEmpty(parametersList);
        // Ensure strategy is not deleted
        StrategyService strategyService = new StrategyService(db, _config.CreateMapper(), NullLogger<StrategyService>.Instance);
        Strategy? s = strategyService.Find(strategyId);
        Assert.NotNull(s);
    }

    [Fact]
    public void Parameters_MultipleStrategies()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddParameters(db, _config);
        int strategyId1 = AddToDb.AddStrategy(db, _config, id);
        int strategyId2 = AddToDb.AddStrategy(db, _config, id);
        ParametersService service = new ParametersService(db, _config.CreateMapper(), NullLogger<ParametersService>.Instance);
        Parameters? parameters = service.GetById(id);
        Assert.NotNull(parameters);
        Assert.NotEmpty(parameters.Strategies);
        Assert.Equal(strategyId1, parameters.Strategies[0].Id);
        Assert.Equal(strategyId2, parameters.Strategies[1].Id);
    }
}