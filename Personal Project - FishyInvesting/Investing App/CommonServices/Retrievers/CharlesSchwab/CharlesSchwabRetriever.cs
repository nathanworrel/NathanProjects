using System.Net.Http.Json;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.MarketTime;
using FishyLibrary.Models.Order;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CommonServices.Retrievers.CharlesSchwab;

public class CharlesSchwabRetriever : ICharlesSchwabRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharlesSchwabRetriever> _logger;

    public CharlesSchwabRetriever(IHttpClientFactory httpClient, string environmentName,
        ILogger<CharlesSchwabRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.CharlesSchwab));
    }

    public void IsLoggedIn()
    {
        IsLoggedInInternal().Wait();
    }

    private async Task IsLoggedInInternal()
    {
        var response = await _httpClient.GetAsync($"EveryoneSignIn");
        _logger.LogInformation("IsLoggedIn at: {time}", DateTimeOffset.Now);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"IsLoggedIn returned status code: {response.StatusCode}");
        }
    }

    public bool IsOpen()
    {
        return IsOpenInternal().Result;
    }

    private async Task<bool> IsOpenInternal()
    {
        HttpResponseMessage response =
            await _httpClient.GetAsync($"IsMarketOpen?accountId=2");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"IsOpen returned status code: {response.StatusCode}");
        }

        var readAsStringAsync = response.Content.ReadAsStringAsync().Result;
        MarketTime? marketTime = JsonSerializer.Deserialize<MarketTime>(readAsStringAsync);
        if (marketTime == null)
        {
            throw new Exception($"Failed to get market time");
        }

        return marketTime.IsOpen && DateTimeOffset.Now.TimeOfDay > marketTime.Open &&
               DateTimeOffset.Now.TimeOfDay < marketTime.Close;
    }

    public AccountInfo? GetAccountInfo(int accountId)
    {
        return GetAccountInfoInternal(accountId).Result;
    }

    private async Task<AccountInfo?> GetAccountInfoInternal(int accountId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"AccountData?accountId={accountId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"GetAccountInfo returned status code: {response.StatusCode}");
        }

        AccountInfo? accountInfo = await response.Content.ReadFromJsonAsync<AccountInfo>();
        return accountInfo;
    }

    public long SendTrade(FishyLibrary.Helpers.MakeTrade makeTrade,
        int accountId, bool dry)
    {
        return SendTradeInternal(makeTrade, accountId, dry).Result;
    }

    private async Task<long> SendTradeInternal(FishyLibrary.Helpers.MakeTrade makeTrade,
        int accountId, bool dry)
    {
        _logger.LogInformation(
            $"Sending trade to account {accountId} with content of {JsonConvert.SerializeObject(makeTrade)}.");
        var httpResponseMessage = await _httpClient.PostAsync($"PlaceTrade?accountId={accountId}&dry={dry}",
            JsonContent.Create(makeTrade));
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"Sending trade failed: {httpResponseMessage.StatusCode}");
        }

        var readAsStringAsync = httpResponseMessage.Content.ReadAsStringAsync().Result;
        if (readAsStringAsync == "")
        {
            return 0;
        }

        return (long)Convert.ToDouble(readAsStringAsync);
    }

    public List<OrderData>? GetOrders(int accountId)
    {
        return GetOrdersInternal(accountId).Result;
    }

    private async Task<List<OrderData>?> GetOrdersInternal(int accountId)
    {
        _logger.LogInformation(
            $"Getting orders for user {accountId}");
        var httpResponseMessage =
            await _httpClient.GetAsync($"Orders?accountId={accountId}&startTime={DateTime.Now.Date:MM/dd/yyyy}");
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"GetOrders returned status code: {httpResponseMessage.StatusCode}");
        }

        var readAsStringAsync = httpResponseMessage.Content.ReadAsStringAsync().Result;

        return JsonSerializer.Deserialize<List<OrderData>>(readAsStringAsync);
    }

    public OrderData? GetOrder(int accountId, long orderId)
    {
        return GetOrderInternal(accountId, orderId).Result;
    }

    private async Task<OrderData?> GetOrderInternal(int accountId, long orderId)
    {
        _logger.LogInformation("Getting order data for account: {AccountId} and order number: {OrderId}", accountId, orderId);
        var response = await _httpClient.GetAsync($"Order?accountId={accountId}&orderNumber={orderId}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"GetOrders returned status code: {response.StatusCode}");
        }

        var utf8Json = response.Content.ReadAsStringAsync().Result;
        return JsonSerializer.Deserialize<OrderData>(utf8Json);
    }

    public List<double> GetCurrentData(List<string> products, int accountId)
    {
        return GetCurrentDataInternal(products, accountId).Result;
    }

    public async Task<List<double>> GetCurrentDataInternal(List<string> products, int accountId)
    {
        List<double> data = new List<double>();
        foreach (var product in products)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(
                $"CurrentMarketPrice?accountId={accountId}&product={product}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"CurrentMarketPrice returned status code {response.StatusCode} and error message {response.ReasonPhrase}");
            }

            data.Add(Double.Parse(response.Content.ReadAsStringAsync().Result));
        }

        return data;
    }
}