using FishyLibrary.Models;

namespace CommonServices.Retrievers.GetData;

public interface IGetDataRetriever
{
    List<StockData> GetData(string startDate, string product);
    List<List<StockData>> GetBacklogDataNum(TimeSpan runTime, List<string> products, int maxPeriod);
    List<List<StockData>> GetBacklogData(TimeSpan runTime, DateTime startDate, List<string> products);
    void CallUpdateData();
    bool LoadDataForProduct(string product);
    Tuple<DateTime?, DateTime?> GetDateRange(string product);
    Task<Tuple<DateTime?, DateTime?>> GetDateRangeInternal(string product);
    Task<bool> LoadDataForProductInternal(string product);
    Task<List<List<StockData>>> GetBacklogDataInternal(TimeSpan runTime, DateTime startDate, List<string> products);
}