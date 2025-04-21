namespace CommonServices.Retrievers.MakeTrade;

public interface IMakeTradeRetriever
{ 
    void Trade(double desiredPosition, double price, int strategyId, bool dry);
}