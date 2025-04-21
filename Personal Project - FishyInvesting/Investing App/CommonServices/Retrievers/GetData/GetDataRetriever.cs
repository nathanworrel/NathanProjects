using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CommonServices.Retrievers.GetData;

public class GetDataRetriever : IGetDataRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GetDataRetriever> _logger;
    private readonly string _environmentName;

    public GetDataRetriever(IHttpClientFactory httpClient, string environmentName, ILogger<GetDataRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.GetData));
        _environmentName = environmentName;
    }

    public List<StockData> GetData(string startDate, string product)
    {
        return GetDataInternal(startDate, product).Result;
    }

    private async Task<List<StockData>> GetDataInternal(string startDate, string product)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"GetData/GetAllDataAtTime" +
            $"?product={product}&time=15%3A00%3A00.000000&startDate={startDate}");
        return JsonConvert.DeserializeObject<List<StockData>>(response.Content.ReadAsStringAsync().Result)!;
    }

    public List<List<StockData>> GetBacklogDataNum(TimeSpan runTime, List<string> products, int maxPeriod)
    {
        return GetBacklogDataNumInternal(runTime, products, maxPeriod).Result;
    }

    private async Task<List<List<StockData>>> GetBacklogDataNumInternal(TimeSpan runTime, List<string> products,
        int maxPeriod)
    {
        string timeOfDayStr = runTime.ToString();
        HttpResponseMessage response = _httpClient.GetAsync(
                $"GetData/GetNumDataAtTimeMultiple?num={maxPeriod}&products={string.Join(",", products)}&time={timeOfDayStr}")
            .Result;
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"GetNumDataAtTimeMultiple returned status code {response.StatusCode} and error message {response.ReasonPhrase}");
        }

        return JsonConvert.DeserializeObject<List<List<StockData>>>(response.Content.ReadAsStringAsync().Result)!;
    }

    public List<List<StockData>> GetBacklogData(TimeSpan runTime, DateTime startDate,
        List<string> products)
    {
        return GetBacklogDataInternal(runTime, startDate, products).Result;
    }

    public async Task<List<List<StockData>>> GetBacklogDataInternal(TimeSpan runTime, DateTime startDate,
        List<string> products)
    {
        string startTimStr = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
        string timeOfDayStr = runTime.ToString();
        HttpResponseMessage response = await _httpClient.GetAsync(
            $"GetData/GetAllDataAtTimeMultiple?products={string.Join(",", products)}&time={timeOfDayStr}&startDate={startTimStr}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"GetAllDataAtTimeMultiple returned status code {response.StatusCode} and error message {response.ReasonPhrase}");
        }

        return JsonConvert.DeserializeObject<List<List<StockData>>>(response.Content.ReadAsStringAsync().Result)!;
    }

    public void CallUpdateData()
    {
        CallUpdateDataInternal().Wait();
    }

    private async Task CallUpdateDataInternal()
    {
        _logger.LogInformation("Calling update data at: {time}", DateTimeOffset.Now);
        HttpResponseMessage response =
            await _httpClient.PatchAsync(
                $"GetData/LoadDataForActiveStrategies", null);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Calling update data failed");
        }
    }

    public bool LoadDataForProduct(string product)
    {
        return LoadDataForProductInternal(product).Result;
    }

    public async Task<bool> LoadDataForProductInternal(string product)
    {
        try
        {
            await _httpClient.PatchAsync($"GetData/LoadDataForProduct?product={product}", null);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to load data for product {product} with reason {reason}", product, e.Message);  
            return false;
        }
        
        return true;
    }

    public Tuple<DateTime?, DateTime?> GetDateRange(string product)
    {
        return GetDateRangeInternal(product).Result;
    }

    public async Task<Tuple<DateTime?, DateTime?>> GetDateRangeInternal(string product)
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(_environmentName, Api.GetData));
        _logger.LogInformation($"Getting date ranges for product {product}");
        HttpResponseMessage response =
            await httpClient.GetAsync(
                $"GetData/DateRanges?product={product}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Calling update data failed");
        }

        return JsonConvert.DeserializeObject<Tuple<DateTime?, DateTime?>>(response.Content.ReadAsStringAsync().Result)!;
    }
}