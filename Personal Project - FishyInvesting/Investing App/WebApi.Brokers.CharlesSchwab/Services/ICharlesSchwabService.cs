using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.MarketTime;
using FishyLibrary.Models.Order;
using WebApi.Template.Models;

namespace WebApi.Template.Services;

public interface ICharlesSchwabService
{
    string? AutomaticSignIn(int userId);
    GenericResponse PlaceTrade(FishyLibrary.Helpers.MakeTrade makeTrade, int accountId, bool dry);
    double GetCurrentMarketPrice(int accountId, string product);
    AccountInfo GetAccountData(int accountId);
    
    MarketTime GetMarketTime(int accountId);
    string? ManualSignIn(int userId);
    void EveryoneSignIn();
    List<OrderData> GetOrders(int accountId, string startTime);
    OrderData GetOrder(int accountId, string orderNumber);
}