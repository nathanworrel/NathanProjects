using System.Globalization;
using FishyLibrary.Models;

namespace WebApi.GetData.Services;

public class DataRequestService
{
    private readonly ILogger<DataRequestService> _logger;
    public DataRequestService(ILogger<DataRequestService> logger)
    {
        _logger = logger;
    }

    private async Task<HttpResponseMessage> RetrieveDataFromSource(string product = "TQQQ", string barInterval = "Minute", int barPeriods = 10,
        string startDate = "2000-01-01", string endDate = "", bool includeExtendedHours = false,
        bool adjustForDividends = true, bool adjustForSplits = true, int startOfDayAdjustmentInMinutes = 0,
        int endOfDayAdjustmentInMinutes = 0, int numberOfMostRecentBars = 0, bool disableCache = true)
    {
        var requestUri = "";//Removed

        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        //removed something here
        using var client = new HttpClient();
        return await client.SendAsync(req);
    }
    
    public List<StockData> ParseCsv(string csvData, string symbol)
    {
        var stockDataList = new List<StockData>();
        string[] lines = csvData.Trim().Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            var stockData = new StockData
            (
                symbol,
                DateTime.Parse(values[2], null, DateTimeStyles.AdjustToUniversal),
                decimal.Parse(values[7], CultureInfo.InvariantCulture)
            );

            stockDataList.Add(stockData);
        }
        _logger.LogInformation($"Parsed {stockDataList.Count} stock data");

        return stockDataList;
    }

    public List<StockData> GetStockData(string product, string startDate)
    {
        var httpResponseMessage = RetrieveDataFromSource(product, startDate: startDate).Result;
        return ParseCsv(httpResponseMessage.Content.ReadAsStringAsync().Result, product);
    }
}