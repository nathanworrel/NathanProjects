using FishyLibrary.Utils;
using FishyLibrary.Models;
using WebApi.GetData.Context;

namespace WebApi.GetData.Services;

public class GetDataService : IGetDataService
{
    private readonly StockDataIntoDatabase _stockDataIntoDatabase;
    private readonly DataRequestService _dataRequestService;
    private readonly ILogger<GetDataService> _logger;
    private readonly GetDataContext _getDataContext;

    public GetDataService(IWebHostEnvironment environment, ILogger<GetDataService> logger,
        GetDataContext getDataContext)
    {
        _logger = logger;

        _stockDataIntoDatabase = new StockDataIntoDatabase(
            DataAccess.GetConnectionString(environment.EnvironmentName)
        );
        _dataRequestService = new DataRequestService(new Logger<DataRequestService>(new LoggerFactory()));
        _getDataContext = getDataContext;
    }

    public void InsertNewData(string product = "TQQQ")
    {
        var data = _dataRequestService.GetStockData(product, GetMostRecentDataTime(product));
        var oldData = _stockDataIntoDatabase.LoadData(product);
        if (oldData.Count > 0)
        {
            data = data.Where(x => oldData.All(y => y.Time.ToUniversalTime() != x.Time)).ToList();
        }

        var count = 0;
        foreach (var subset in data.Select((value, index) => new { value, index })
                     .GroupBy(x => x.index / 100)
                     .Select(g => g.Select(x => x.value).ToList())
                     .ToList())
        {
            count += subset.Count;
            _stockDataIntoDatabase.InsertAllData(subset);
        }

        _logger.LogInformation($"Stored data for {product}: {count}");
    }

    private StockData? GetRecentDataFromDatabase(string product)
    {
        return _stockDataIntoDatabase.LoadRecentData(product);
    }

    private string GetMostRecentDataTime(string product)
    {
        var recentData = GetRecentDataFromDatabase(product);
        if (recentData != null)
        {
            return recentData.Time.ToString("yyyy-MM-dd");
        }

        return "2000-01-01";
    }

    public List<StockData> GetDataAtTimeAfter(string product, DateTime time, DateTime starTime)
    {
        return _stockDataIntoDatabase.LoadDataAtTime(product, time, starTime);
    }

    public void InsertNewDataForActiveStrategies()
    {
        List<int> activeStrategyNumbers =
            _getDataContext.strategies.Where(x => x.Active == true).Select(x => x.Id).ToList();
        List<string> primaryProducts =
            _getDataContext.strategies.Where(x => x.Active == true).Select(x => x.Product).ToList();
        List<string> secondaryProducts = _getDataContext.SecondaryProducts
            .Where(x => activeStrategyNumbers.Contains(x.StrategyId)).Select(x => x.Product).ToList();
        HashSet<string> products = new HashSet<string>();
        products.UnionWith(primaryProducts);
        products.UnionWith(secondaryProducts);
        foreach (var product in products)
        {
            InsertNewData(product);
        }
    }

    public Tuple<DateTime?, DateTime?> GetDateRange(string product)
    {
        return new Tuple<DateTime?, DateTime?>(
            _stockDataIntoDatabase.GetMinTime(product), _stockDataIntoDatabase.GetMaxTime(product)
        );
    }
}