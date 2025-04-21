using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategySecondaryProduct;
using FishyLibrary.Models.StrategyType;
using FishyLibrary.Models.Trade;
using FishyLibrary.Models.User;
using Microsoft.Extensions.Logging.Abstractions;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace UnitTests.Utils;

public class AddToDb
{
    public static int AddStrategyType(DBAccessContext db, MapperConfiguration config, int id = 1)
    {
        StrategyTypePost strategyType = new StrategyTypePost(id, "Strategy Type");
        StrategyTypeService service =
            new StrategyTypeService(db, config.CreateMapper(), NullLogger<StrategyTypeService>.Instance);
        if (service.GetById(id) is null)
        {
            return service.AddStrategyType(strategyType);   
        }
        return id;
    }

    public static int AddParameters(DBAccessContext db, MapperConfiguration config, int stratId = 1)
    {
        var service = new ParametersService(db, config.CreateMapper(), NullLogger<ParametersService>.Instance);
        var dict = new Dictionary<string, double?>
        {
            { "temp", 1.0 },
        };
        ParametersPost param = new ParametersPost(0, dict, AddStrategyType(db, config, stratId));
        return service.AddParameters(param);
    }

    public static int AddUser(DBAccessContext db, MapperConfiguration config)
    {
        UserPost user = new UserPost("temp user", "temp password", false);
        UserService service = new UserService(db, config.CreateMapper(), NullLogger<UserService>.Instance);
        return service.AddUser(user);
    }
    
    public static int AddAccount(DBAccessContext db, MapperConfiguration config)
    {
        return AddAccount(db, config, AddUser(db, config));
    }

    public static int AddAccount(DBAccessContext db, MapperConfiguration config, int userId)
    {
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, config.CreateMapper());
        AccountPost accountPost = new AccountPost(0, "account id", "hash account id", userId);
        return service.AddAccount(accountPost);
    }
    
    public static int AddStrategy(DBAccessContext db, MapperConfiguration config, int paramId)
    {
        return AddStrategy(db, config, paramId, AddAccount(db, config));
    }

    public static int AddStrategy(DBAccessContext db, MapperConfiguration config, int paramId, int accountId)
    {
        StrategyService service = new StrategyService(db, config.CreateMapper(), NullLogger<StrategyService>.Instance);
        StrategyPost strategyPost = new StrategyPost(0, "test strategy", "this is a test", 
            "QQQ", false, "", 100, true, 0, paramId, 
            DateTime.Now, accountId);
        return service.AddStrategy(strategyPost);
    }
    
    public static int AddStrategy(DBAccessContext db, MapperConfiguration config)
    {
        return AddStrategy(db, config, AddParameters(db, config), AddAccount(db, config));
    }

    public static int AddTrade(DBAccessContext db, MapperConfiguration config)
    {
        return AddTrade(db, config, AddStrategy(db, config));
    }

    public static int AddTrade(DBAccessContext db, MapperConfiguration config, int strategyId)
    {
        TradeService service = new TradeService(NullLogger<TradeService>.Instance, db, config.CreateMapper());
        TradePost tradePost = new TradePost(new DateTime(2024, 12, 14), 100, 100, (int) Side.BUY, strategyId, 1);
        return service.Add(tradePost);
    }

    public static int AddStrategyRuntime(DBAccessContext db, MapperConfiguration config)
    {
        return AddStrategyRuntime(db, config, AddStrategy(db, config));
    }

    public static int AddStrategyRuntime(DBAccessContext db, MapperConfiguration config, int strategyId)
    {
        StrategyRuntimeService service = new StrategyRuntimeService(NullLogger<StrategyRuntimeService>.Instance, db, config.CreateMapper());
        return service.Add(strategyId);
    }

    public static int AddStrategySecondaryProduct(DBAccessContext db, MapperConfiguration config)
    {
        return AddStrategySecondaryProduct(db, config, AddStrategy(db, config));
    }

    public static int AddStrategySecondaryProduct(DBAccessContext db, MapperConfiguration config, int strategyId)
    {
        StrategySecondaryProductsService service = 
            new StrategySecondaryProductsService(NullLogger<StrategySecondaryProductsService>.Instance, db, config.CreateMapper());
        StrategySecondaryProductPost post = new StrategySecondaryProductPost(0, "QQQ", strategyId, 1);
        return service.Add(post);
    }
}