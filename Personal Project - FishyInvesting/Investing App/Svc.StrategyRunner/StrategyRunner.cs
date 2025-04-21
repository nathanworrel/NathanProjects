using CommonServices.Retrievers.CharlesSchwab;
using CommonServices.Retrievers.GetData;
using CommonServices.Retrievers.Scheduler;
using CommonServices.Retrievers.UpdateTrades;
using FishyLibrary.Utils;

namespace Svc.StrategyRunner;

public class StrategyRunner : IHostedService, IDisposable
{
    private readonly ILogger<StrategyRunner> _logger;
    private Timer? _timer;
    private static readonly int _interval = 10;
    private readonly ICharlesSchwabRetriever _charlesSchwabRetriever;
    private readonly IGetDataRetriever _getDataRetriever;
    private readonly IUpdateTradesRetriever _updateTradesRetriever;
    private readonly ISchedulerRetriever _schedulerRetriever;

    public StrategyRunner(ILogger<StrategyRunner> logger, ICharlesSchwabRetriever charlesSchwabRetriever,
        IGetDataRetriever getDataRetriever, IUpdateTradesRetriever updateTradesRetriever,
        ISchedulerRetriever schedulerRetriever)
    {
        _logger = logger;
        _charlesSchwabRetriever = charlesSchwabRetriever;
        _getDataRetriever = getDataRetriever;
        _updateTradesRetriever = updateTradesRetriever;
        _schedulerRetriever = schedulerRetriever;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Strategies starting at: {time}", DateTimeOffset.Now);
        _getDataRetriever.CallUpdateData();
        _charlesSchwabRetriever.IsLoggedIn();
        _ = _charlesSchwabRetriever.IsOpen();
        TimeSpan dueTime = TimeHelpers.GetTimeToInterval(DateTime.Now, _interval);
        _timer = new Timer(Execute, null, dueTime, new TimeSpan(0, 0, _interval, 0));
        return Task.CompletedTask;
    }

    private void Execute(object? state)
    {
        var roundToNearestInterval = TimeHelpers.RoundToNearestInterval(DateTime.Now.TimeOfDay, _interval);
        _logger.LogInformation("Strategies running at: {time}, runtime:{time}", DateTimeOffset.Now,
            roundToNearestInterval);

        if (TimeHelpers.RoundUp(DateTime.Now, TimeSpan.FromMinutes(_interval)).Hour == 6
            && TimeHelpers.IsMorning(DateTime.Now))
        {
            _getDataRetriever.CallUpdateData();
            _charlesSchwabRetriever.IsLoggedIn();
        }

        if (_charlesSchwabRetriever.IsOpen())
        {
            _schedulerRetriever.CallScheduler(roundToNearestInterval);
            _updateTradesRetriever.CallUpdateTrades();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Strategies stopping at: {time}", DateTimeOffset.Now);
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}