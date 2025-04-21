using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using WebApi.DB.Access.Contexts;

namespace UnitTests;

public class TestDatabaseFixture : IDisposable
{
    private readonly object _lock = new();
    private readonly PostgreSqlContainer _postgresContainer;

    public TestDatabaseFixture()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("1")
            .WithPassword("1")
            .Build();
        
        _postgresContainer.StartAsync().Wait();    
    }

    public void Dispose()
    {
        _postgresContainer.DisposeAsync();
    }

    public void Reset()
    {
        using (var context = CreateContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        Monitor.Exit(_lock);
    }

    public DBAccessContext CreateContext()
    {
        Monitor.Enter(_lock);
        var context =  new DBAccessContext(
            new DbContextOptionsBuilder<DBAccessContext>()
                .UseNpgsql(_postgresContainer.GetConnectionString())
                .Options, null);
        context.Database.EnsureCreated();
        return context;
    }
}