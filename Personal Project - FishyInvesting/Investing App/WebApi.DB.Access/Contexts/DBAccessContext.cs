using FishyLibrary;
using FishyLibrary.Utils;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.StrategyRuntime;
using FishyLibrary.Models.StrategySecondaryProduct;
using FishyLibrary.Models.StrategyType;
using FishyLibrary.Models.Trade;
using FishyLibrary.Models.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebApi.DB.Access.Contexts;

public class DBAccessContext : DbContext
{
    public DbSet<Client> Users { get; set; }
    public DbSet<StrategyType> StrategyTypes { get; set; }
    public DbSet<Parameters> Parameters { get; set; }
    public DbSet<Strategy> Strategies { get; set; }
    public DbSet<StrategySecondaryProduct> SecondaryProducts { get; set; }
    public DbSet<StrategyRuntime> StrategyRuntimes { get; set; }
    public DbSet<Trade> Trades { get; set; }
    
    public DbSet<Account> Accounts { get; set; }
    
    private readonly IWebHostEnvironment? _env;

    public DBAccessContext(DbContextOptions<DBAccessContext> options, IWebHostEnvironment env) : base(options)
    {
        _env = env;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (!builder.IsConfigured)
        {
            if (_env is not null)
            {
                string connectionString = DataAccess.GetConnectionString(_env.EnvironmentName);
                builder.UseNpgsql(connectionString);   
            }
        }
    
        base.OnConfiguring(builder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Parameters>()
            .Property(e => e.Params)
            .HasColumnType("json")
            .HasConversion(
                v => JsonConvert.SerializeObject(v), 
                v => JsonConvert.DeserializeObject<Dictionary<string, double?>>(v));
        
        modelBuilder
            .Entity<Parameters>()
            .HasMany(e => e.Strategies)
            .WithOne(e => e.Parameters)
            .HasForeignKey(e => e.ParameterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // Restrict deletion
        
        modelBuilder
            .Entity<Parameters>()
            .HasOne<StrategyType>(e => e.StrategyType)
            .WithMany(e => e.Parameters)
            .HasForeignKey(e => e.StrategyTypeId)
            .IsRequired();

        modelBuilder
            .Entity<Strategy>()
            .HasMany<StrategyRuntime>(e => e.Runtimes)
            .WithOne(e => e.Strategy)
            .HasForeignKey(e => e.StrategyId);
        
        modelBuilder
            .Entity<Strategy>()
            .HasMany<StrategySecondaryProduct>(e => e.SecondaryProducts)
            .WithOne(e => e.Strategy)
            .HasForeignKey(e => e.StrategyId);
        
        modelBuilder
            .Entity<Strategy>()
            .HasMany<Trade>(e => e.Trades)
            .WithOne(e => e.Strategy)
            .HasForeignKey(e => e.StrategyId);
        
        modelBuilder
            .Entity<Strategy>()
            .Property(e => e.IntermediateData)
            .HasColumnType("json")
            .HasConversion(
                v => JsonConvert.SerializeObject(v), 
                v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v));

        modelBuilder
            .Entity<Account>()
            .HasOne<Client>(e => e.Client)
            .WithMany(e => e.Accounts)
            .HasForeignKey(e => e.ClientId);
        
        modelBuilder
            .Entity<Strategy>()
            .HasOne<Account>(e => e.Account)
            .WithMany(e => e.Strategies)
            .HasForeignKey(e => e.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.ToSnakeCase();
    }
}