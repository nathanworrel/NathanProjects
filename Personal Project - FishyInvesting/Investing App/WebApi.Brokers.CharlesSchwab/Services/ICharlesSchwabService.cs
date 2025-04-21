using FishyLibrary.Models;
using FishyLibrary.Models.MarketTime;
using WebApi.Template.Models;

namespace WebApi.Template.Services;

public interface ICharlesSchwabService
{
    string? AutomaticSignIn(int userId);
    Task<HttpResponseMessage> SendMakeTrade(Order order,  int accountId);
    Task<HttpResponseMessage> VerifySendMakeTrade(Order order,  int accountId);
    Task<HttpResponseMessage> GetCurrentMarketPrice(int accountId, string product);
    Task<HttpResponseMessage> GetAccountData(int accountId);
    
    Task<MarketTime> GetMarketTime(int accountId);
    string? ManualSignIn(int userId);
    void EveryoneSignIn();
    Task<HttpResponseMessage> GetOrders(int accountId, DateTime startDate);
    Task<HttpResponseMessage> GetOrder(int accountId, string orderNumber);
}