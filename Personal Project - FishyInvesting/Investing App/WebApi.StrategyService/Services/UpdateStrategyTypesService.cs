using FishyLibrary.Enums;
using FishyLibrary.Models;
using FishyLibrary.Models.StrategyType;
using WebApi.StrategyService.Contexts;

namespace WebApi.StrategyService.Services;

public class UpdateStrategyTypesService : BackgroundService
{
    private ILogger<UpdateStrategyTypesService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public UpdateStrategyTypesService(ILogger<UpdateStrategyTypesService> logger,  IServiceScopeFactory options)
    {
        _logger = logger;
        _scopeFactory  = options;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start update strategy types service");
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StrategyServiceContext>();
        foreach (StrategyName name in Enum.GetValues(typeof(StrategyName)))
        {
            StrategyType? strategyType = context.StrategyTypes.Find((int) name);
            if (strategyType == null)
            {
                StrategyType strategy = new StrategyType((int) name, name.ToString());
                context.StrategyTypes.Add(strategy);
            }
            else
            {
                strategyType.Name = name.ToString();
            }
            context.SaveChanges();
        }
        return Task.CompletedTask;
    }
}