using FishyLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.GetData.Services;

namespace WebApi.GetData.Controllers;

[ApiController]
[Route("[controller]")]
public class GetDataController : ControllerBase
{
    private readonly ILogger<GetDataController> _logger;
    private readonly IGetDataService _getDataService;

    public GetDataController(ILogger<GetDataController> logger, IGetDataService service)
    {
        _logger = logger;
        _getDataService = service;
    }

    [HttpGet("GetAllDataAtTime")]
    public List<StockData> GetAllDataAtTime(string product = "TQQQ", string time = "9:40:00.000000",
        string startDate = "2000-01-01")
    {
        return GetDataAtTimeAfter(product, time, startDate);
    }

    [HttpGet("GetNumDataAtTime")]
    public List<StockData> GetNumData(int num, string product = "TQQQ", string time = "9:40:00.000000")
    {
        _logger.LogInformation("Get {num} stock data at time: {time}",  num, time);
        return GetDataAtTimeAfter(product, time, DateTime.Now.AddDays(num * -2 - 10).Date.ToString()).TakeLast(num)
            .ToList();
    }

    [HttpGet("GetAllDataAtTimeMultiple")]
    public List<List<StockData>> GetAllDataAtTimeMultiple(string products = "TQQQ,$VIX",
        string time = "10:40:00.000000",
        string startDate = "2000-01-01")
    {
        _logger.LogInformation("Get all stock data for {products} at time: {time} after {startDate}", products, time, startDate);
        return GetDataAtTimeAfterMultiple(products, time, startDate);
    }

    [HttpGet("GetNumDataAtTimeMultiple")]
    public List<List<StockData>> GetNumDataMultiple(int num, string products = "TQQQ,$VIX",
        string time = "10:40:00")
    {
        try
        {
            _logger.LogInformation("Get {num} stock data at time {time}  for {products}", num, time, products);
            List<List<StockData>> data =
                GetDataAtTimeAfterMultiple(products, time, DateTime.Now.AddDays(num * -2 - 10).Date.ToString());
            for (var i = 0; i < data.Count; i++)
            {
                data[i] = data[i].TakeLast(num).ToList();
            }

            return data;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("DateRanges")]
    public Tuple<DateTime?, DateTime?> GetDateRanges(string product)
    {
        _logger.LogInformation("Get {product} data ranges", product);
        var result = _getDataService.GetDateRange(product);
        _logger.LogInformation("Date Range for product is {result.Item1} - {result.Item2}", result.Item1, result.Item2);
        return result;
    }

    [HttpPatch("LoadDataForProduct")]
    public void LoadDataForProduct(string product = "TQQQ")
    {
        _logger.LogInformation("Load data for product: {product}", product);
        _getDataService.InsertNewData(product);
    }

    [HttpPatch("LoadDataForActiveStrategies")]
    public void LoadDataForActiveStrategies()
    {
        _logger.LogInformation("Load data for active strategies");
        _getDataService.InsertNewDataForActiveStrategies();
    }

    private List<List<StockData>> GetDataAtTimeAfterMultiple(string products, string time, string startDate)
    {
        var listProducts = products.Split(',').ToList();

        var listStockData = new List<List<StockData>>();

        foreach (var product in listProducts)
        {
            var dataAtTimeAfter = GetDataAtTimeAfter(product, time, startDate);
            if (dataAtTimeAfter.Count == 0)
            {
                return new List<List<StockData>>();
            }
            listStockData.Add(dataAtTimeAfter);
        }

        var later = GetLatestDate(listStockData);

        for (var i = 0; i < listStockData.Count; i++)
        {
            listStockData[i] = listStockData[i].Where(p => p.Time > later).ToList();
        }

        for (var i = 0; i < listStockData.Count - 1; i++)
        {
            if (listStockData[i].Count != listStockData[i + 1].Count)
            {
                throw new Exception(
                    $"Mismatch number of stock data {i}: {listStockData[i].Count} and {i + 1}: {listStockData[i + 1].Count}");
            }
        }

        return listStockData;
    }

    private List<StockData> GetDataAtTimeAfter(string product, string time, string startDate)
    {
        DateTime utcTime = GetUTCTime(time);
        _logger.LogInformation("Get all stock data UTC at time: {utcTime}", utcTime);
        return _getDataService.GetDataAtTimeAfter(product, utcTime,
            startDate != "" ? DateTime.Parse(startDate) : DateTime.MinValue);
    }

    private DateTime GetUTCTime(string time)
    {
        TimeOnly timeOnly = TimeOnly.Parse(time);
        DateOnly dateOnly = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        DateTime localTime = new DateTime(dateOnly, timeOnly, DateTimeKind.Unspecified);
        return localTime.ToUniversalTime();
    }

    private DateTime GetLatestDate(List<List<StockData>>? listStockData)
    {
        var later = listStockData.First().First().Time;

        foreach (var stockData in listStockData)
        {
            if (stockData.First().Time > later)
            {
                later = stockData.First().Time;
            }
        }

        return later;
    }
}