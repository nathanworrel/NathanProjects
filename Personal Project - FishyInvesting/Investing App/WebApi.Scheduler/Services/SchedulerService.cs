using CommonServices.Retrievers.StrategyService;
using WebApi.GetData.Context;

namespace WebApi.Scheduler.Services;

public class SchedulerService : ISchedulerService
{
    private readonly ILogger<SchedulerService> _logger;
    private readonly SchedulerContext _schedulerContext;
    private readonly IStrategyServiceRetriever _strategyServiceRetriever;

    public SchedulerService(ILogger<SchedulerService> logger, SchedulerContext schedulerContext, IStrategyServiceRetriever strategyServiceRetriever)
    {
        _logger = logger;
        _schedulerContext = schedulerContext;
        _strategyServiceRetriever = strategyServiceRetriever;
    }

    public void StartScheduler(TimeSpan runTime)
    {
        List<int> runningStrategyIds = _schedulerContext.strategyRuntimes.Where(x => x.Runtime.Equals(runTime))
            .Select(x => x.StrategyId).ToList();
        List<int> activeRunningStrategyIds = _schedulerContext.strategies
            .Where(x => runningStrategyIds.Contains(x.Id) && x.Active).Select(x => x.Id).Distinct().ToList();
        foreach (var activeRunningStrategyId in activeRunningStrategyIds)
        {
            try
            {
                _strategyServiceRetriever.SendToStrategyService(runTime, activeRunningStrategyId);
            }
            catch (Exception e)
            {
                _logger.LogError("{}", e);
            }
        }
    }
}