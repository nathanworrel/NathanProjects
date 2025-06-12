using FishyLibrary.Models;
using FishyLibrary.Models.Order;
using WebApi.Template.Models;

namespace WebApi.Template.Services;

public interface ICharlesSchwabDataService
{
    AuthResult? SendRefresh(Dictionary<string, string> dict, string appKey, string appSecret);
    GenericResponse SendMakeTrade(Order order, string accountHashValue, string accessToken);
    GenericResponse VerifySendMakeTrade(Order order, string accountHashValue, string accessToken);
    PriceData GetCurrentMarketPrice(string product, string accessToken);
    CSAccountData GetAccountData(string accountHashValue, string accessToken);
    List<AccountResponse> GetAccountHash(string accessToken);
    EquityResponse IsMarketOpen(string accessToken);
    List<OrderData> GetOrders(string accountHashValue, string accessToken, DateTime startDate);
    OrderData GetOrder(string accountHashValue, string accessToken, string orderNumber);
}