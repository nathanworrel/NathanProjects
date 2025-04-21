using FishyLibrary.Models.Trade;

namespace MakeTrade.Services;

public interface IMakeTradeService
{ 
    Trade? MakeTrade(float desiredPosition, int strategyId, double currentPrice, bool dry);
}