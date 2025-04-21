using AutoMapper;
using FishyLibrary.Models;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.User;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UnitTests.Utils;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace UnitTests.WebApi.DB.Access.Services;

[Collection("Non-Parallel Tests")]
public class AccountServiceTest : IClassFixture<TestDatabaseFixture>, IDisposable
{
    private readonly MapperConfiguration _config = 
        new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
    private TestDatabaseFixture Fixture { get; }

    public AccountServiceTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    public void Dispose()
    {
        Fixture.Reset();
    }
    
    [Fact]
    public void AccountService_GetAllAccounts_Empty()
    {
        using var db = Fixture.CreateContext();
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        var accounts = service.GetAllAccounts();
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void AccountService_AddAccount_Invalid_User()
    {
        AccountPost accountPost = new AccountPost(1, "account id", "hash account id", 10000);
        using var db = Fixture.CreateContext();
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        int id = service.AddAccount(accountPost);
        Assert.Equal(0, id);
        var accounts = service.GetAllAccounts();
        Assert.Empty(accounts);
    }
    
    [Fact]
    public void AccountService_AddAccount()
    {
        using var db = Fixture.CreateContext();
        int receivedUserId = AddToDb.AddUser(db, _config);
        AccountPost accountPost = new AccountPost(1, "account id", "hash account id", receivedUserId);
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        int id = service.AddAccount(accountPost);
        Assert.NotEqual(0, id);
        var accounts = service.GetAllAccounts();
        Assert.NotEmpty(accounts);
        Assert.Single(accounts);
    }

    [Fact]
    public void AccountService_Null_AccountId()
    {
        using var db = Fixture.CreateContext();
        int receivedUserId = AddToDb.AddUser(db, _config);
        AccountPost accountPost = new AccountPost(1, null, "hash account id", receivedUserId);
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        int id = service.AddAccount(accountPost);
        Assert.Equal(0, id);
    }
    
    [Fact]
    public void AccountService_Null_HashAccountId()
    {
        using var db = Fixture.CreateContext();
        int receivedUserId = AddToDb.AddUser(db, _config);
        AccountPost accountPost = new AccountPost(1, "account id", null, receivedUserId);
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        int id = service.AddAccount(accountPost);
        Assert.Equal(0, id);
    }

    [Fact]
    public void AccountService_Delete()
    {
        using var db = Fixture.CreateContext();
        int id = AddToDb.AddAccount(db, _config);
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        var account = service.Find(id);
        Assert.NotNull(account);
        var accountPost = service.Delete(account);
        Assert.NotNull(accountPost);
    }
    
    [Fact]
    public void AccountService_Delete_WithStrategies()
    {
        using var db = Fixture.CreateContext();
        int paramId = AddToDb.AddParameters(db, _config);
        int id = AddToDb.AddAccount(db, _config);
        
        AddToDb.AddStrategy(db, _config, paramId, id);
        AccountService service = new AccountService(NullLogger<AccountService>.Instance, db, _config.CreateMapper());
        var account = service.Find(id);
        Assert.NotNull(account);
        var accountPost = service.Delete(account);
        Assert.Null(accountPost);
    }
}