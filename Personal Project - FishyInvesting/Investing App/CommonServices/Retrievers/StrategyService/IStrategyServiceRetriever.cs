namespace CommonServices.Retrievers.StrategyService;

public interface IStrategyServiceRetriever
{
    void SendToStrategyService(TimeSpan timeSpan, int strategyId);
}