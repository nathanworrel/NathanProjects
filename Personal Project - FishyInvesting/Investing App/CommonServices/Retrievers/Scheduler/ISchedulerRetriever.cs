namespace CommonServices.Retrievers.Scheduler;

public interface ISchedulerRetriever
{ 
    void CallScheduler(TimeSpan time);
}