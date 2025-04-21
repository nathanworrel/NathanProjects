using FishyLibrary.Models;

namespace WebApi.GetData.Services;

public interface IGetDataService
{
    List<StockData> GetDataAtTimeAfter(string product, DateTime time, DateTime starTime);

    void InsertNewData(string product);
    void InsertNewDataForActiveStrategies();
    Tuple<DateTime?, DateTime?> GetDateRange(string product);
}