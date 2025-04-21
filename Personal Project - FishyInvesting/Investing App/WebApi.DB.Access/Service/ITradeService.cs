using FishyLibrary.Models.Trade;

namespace WebApi.DB.Access.Service;

public interface ITradeService
{
    TradeGet? Get(int id);
    Trade? Find(int id);
    List<TradeGet> GetAll();
    int Add(TradePost trade);
    int Edit(Trade trade, TradePost tradePost);
    TradePost Delete(Trade trade);
}