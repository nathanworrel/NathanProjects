using FishyLibrary.Models;
using FishyLibrary.Models.Trade;

namespace WebApi.DB.Access.Service;

public interface IEarningService
{
    List<Return> GenerateReturns(List<StockData> prices, List<Trade> trades);
    decimal GetTotalProfits(List<Trade> trades);
    List<Return> ConsolidateReturns(List<List<Return>> returns);
}