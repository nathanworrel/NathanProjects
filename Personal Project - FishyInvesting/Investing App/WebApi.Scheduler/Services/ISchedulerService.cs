namespace WebApi.Scheduler.Services;

public interface ISchedulerService
{
    void StartScheduler(TimeSpan runTime);
}