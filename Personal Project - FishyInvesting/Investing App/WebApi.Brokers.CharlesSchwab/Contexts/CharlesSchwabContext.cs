using FishyLibrary;
using FishyLibrary.Models.MarketTime;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Template.Models;
using Account = FishyLibrary.Models.Account.Account;

namespace WebApi.Template.Contexts;

public class CharlesSchwabContext : DbContext
{
    public DbSet<AuthToken> AuthTokens { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<MarketTime> MarketTimes { get; set; }
    private IWebHostEnvironment _env;

    public CharlesSchwabContext(DbContextOptions<CharlesSchwabContext> options, IWebHostEnvironment environment) : base(options)
    {
        _env = environment;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (!builder.IsConfigured)
        {
            string host = "localhost";
            int port = 1;
            string database = "dev-1";
            if (_env != null && !_env.IsDevelopment())
            {
                
                host = "db";
                port = 1;
                database = "1";
            }
            builder.UseNpgsql($"Host={host};Port={port};Database={database};User Id=postgres;Password=1;");
        }

        base.OnConfiguring(builder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Strategy>()
            .HasOne(s => s.Parameters)       // A Strategy has one Parameters
            .WithMany(p => p.Strategies)     // Parameters have many Strategies
            .HasForeignKey(s => s.ParameterId); // Set foreign key in Strategy

        modelBuilder
            .Entity<Parameters>()
            .Property(e => e.Params)
            .HasColumnType("json")
            .HasConversion(
                v => JsonConvert.SerializeObject(v), 
                v => JsonConvert.DeserializeObject<Dictionary<string, double?>>(v));
        
        modelBuilder
            .Entity<Strategy>()
            .Property(e => e.IntermediateData)
            .HasColumnType("json")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v));
        modelBuilder.ToSnakeCase();
    }
}