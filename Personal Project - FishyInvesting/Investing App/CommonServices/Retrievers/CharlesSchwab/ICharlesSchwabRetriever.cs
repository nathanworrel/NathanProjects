using FishyLibrary.Helpers;
using FishyLibrary.Models.Order;

namespace CommonServices.Retrievers.CharlesSchwab;

public interface ICharlesSchwabRetriever
{
     void IsLoggedIn();
     bool IsOpen();
     AccountInfo? GetAccountInfo(int accountId);
     long SendTrade(FishyLibrary.Helpers.MakeTrade makeTrade,
        int accountId, bool dry);
     List<OrderData>? GetOrders(int accountId);
     OrderData? GetOrder(int accountId, long orderId);
     List<double> GetCurrentData(List<string> products, int accountId);
}