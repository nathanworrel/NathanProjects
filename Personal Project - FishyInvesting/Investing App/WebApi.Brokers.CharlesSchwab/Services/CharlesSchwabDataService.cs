using System.Net.Http.Headers;
using System.Text;
using FishyLibrary.Models;
using FishyLibrary.Models.Order;
using Newtonsoft.Json;
using WebApi.Template.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApi.Template.Services;

public class CharlesSchwabDataService : ICharlesSchwabDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharlesSchwabDataService> _logger;
    private static readonly string RefreshAccessTokenUrl = "https://api.schwabapi.com/v1/oauth/token";
    private static readonly string AppCallbackUrl = "https://127.0.0.1";
    private static readonly string TraderBaseUrl = "https://api.schwabapi.com/trader/v1";
    private static readonly string MarketDataBaseUrl = "https://api.schwabapi.com/marketdata/v1";
    
    public CharlesSchwabDataService(IHttpClientFactory httpClientFactory,
        ILogger<CharlesSchwabDataService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }
    
    public AuthResult? SendRefresh(Dictionary<string, string> dict, string appKey, string appSecret)
    {
        var response = SendRefreshInternal(dict, appKey, appSecret).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            return JsonSerializer.Deserialize<AuthResult>(responseBody);
        }

        _logger.LogError("Failed to get account data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }
    

    private async Task<HttpResponseMessage> SendRefreshInternal(Dictionary<string, string> dict, string appKey, string appSecret)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, RefreshAccessTokenUrl);
        string credentials = $"{appKey}:{appSecret}";
        string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        req.Headers.Add("Authorization", $"Basic {base64Credentials}");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        req.Content = new FormUrlEncodedContent(dict);
        return await _httpClient.SendAsync(req);
    }

    public GenericResponse SendMakeTrade(Order order, string accountHashValue, string accessToken)
    {
        GenericResponse genericResponse = new GenericResponse();
        _logger.LogInformation("Sending Order: {order}", order);
        var httpResponseMessage = SendMakeTradeInternal(order, accountHashValue, accessToken).Result;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            genericResponse.Status = "Success";
            genericResponse.Message = httpResponseMessage.Headers.Location.ToString().Split('/').Last();
            genericResponse.isSuccess = true;
        }
        else
        {
            genericResponse.Status = "Error";
            genericResponse.Message = httpResponseMessage.Content.ReadAsStringAsync().Result;
            genericResponse.isSuccess = false;
            _logger.LogError("Error Sending Order: {order}, response: {response}", order, httpResponseMessage);
        }
        _logger.LogInformation("Sending Order Response: {response}", genericResponse);

        return genericResponse;
    }

    private async Task<HttpResponseMessage> SendMakeTradeInternal(Order order, string accountHashValue, string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Post,
            $"{TraderBaseUrl}/accounts/{accountHashValue}/orders");
        var jsonOrder = JsonConvert.SerializeObject(order);
        req.Content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
    
    public GenericResponse VerifySendMakeTrade(Order order, string accountHashValue, string accessToken)
    {
        GenericResponse genericResponse = new GenericResponse();
        _logger.LogInformation("Sending Order: {order}", order);
        var httpResponseMessage = VerifySendMakeTradeInternal(order, accountHashValue, accessToken).Result;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            genericResponse.Status = "Success";
            genericResponse.Message = httpResponseMessage.Content.ReadAsStringAsync().Result;
            genericResponse.isSuccess = true;
        }
        else
        {
            genericResponse.Status = "Error";
            genericResponse.Message = httpResponseMessage.Content.ReadAsStringAsync().Result;
            genericResponse.isSuccess = false;
            _logger.LogError("Error Sending Order: {order}, response: {response}", order, httpResponseMessage);
        }
        _logger.LogInformation("Sending Order Response: {response}", genericResponse);

        return genericResponse;
    }

    private async Task<HttpResponseMessage> VerifySendMakeTradeInternal(Order order, string accountHashValue, string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Post,
            $"{TraderBaseUrl}/accounts/{accountHashValue}/previewOrder");
        var jsonOrder = JsonConvert.SerializeObject(order);
        req.Content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
    
    public PriceData GetCurrentMarketPrice(string product, string accessToken)
    {
        var response = GetCurrentMarketPriceInternal(product, accessToken).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<Dictionary<string,PriceData>>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to get account data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data[product];
        }

        _logger.LogError("Failed to get account data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }

    private async Task<HttpResponseMessage> GetCurrentMarketPriceInternal(string product, string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{MarketDataBaseUrl}/{product}/quotes");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
    
    public CSAccountData GetAccountData(string accountHashValue, string accessToken)
    {
        var response = GetAccountDataInternal(accountHashValue, accessToken).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<CSAccountData>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to get account data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data;
        }

        _logger.LogError("Failed to get account data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }
    
    private async Task<HttpResponseMessage> GetAccountDataInternal(string accountHashValue, string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{accountHashValue}?fields=positions");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
    
    public List<AccountResponse> GetAccountHash(string accessToken)
    {
        var response = GetAccountHashInternal(accessToken).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<List<AccountResponse>>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to get account hash data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data;
        }

        _logger.LogError("Failed to get account hash data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }

    private async Task<HttpResponseMessage> GetAccountHashInternal(string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/accountNumbers");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
    
    public EquityResponse IsMarketOpen(string accessToken)
    {
        var response = IsMarketOpenInternal(accessToken).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<EquityResponse>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to parse market data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data;
        }

        _logger.LogError("Failed to get market data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }

    private async Task<HttpResponseMessage> IsMarketOpenInternal(string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{MarketDataBaseUrl}/markets/equity?date={DateTime.Now.Date:yyyy-MM-dd}");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }

    public List<OrderData> GetOrders(string accountHashValue, string accessToken, DateTime startDate)
    {
        var response = GetOrdersInternal(accountHashValue, accessToken, startDate).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<List<OrderData>>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to parse orders data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data;
        }

        _logger.LogError("Failed to get orders data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }

    private async Task<HttpResponseMessage> GetOrdersInternal(string accountHashValue, string accessToken, DateTime startDate)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{accountHashValue}/orders?fromEnteredTime={startDate.ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'").Replace(":", "%3A")}&toEnteredTime={DateTime.Now.Date.AddDays(1).Date.ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'").Replace(":", "%3A")}");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }

    public OrderData GetOrder(string accountHashValue, string accessToken, string orderNumber)
    {
        var response = GetOrderInternal(accountHashValue, accessToken, orderNumber).Result;
        if (response.IsSuccessStatusCode)
        {
            var responseBody = response.Content.ReadAsStringAsync().Result;
            _logger.LogDebug("{responseBody}", responseBody);
            var data = JsonSerializer.Deserialize<OrderData>(responseBody);
            if (data == null)
            {
                _logger.LogError("Failed to parse order data from {response}", responseBody);
                throw new NullReferenceException();
            }
            return data;
        }

        _logger.LogError("Failed to get order data; {response}", response);
        throw new Exception(response.ReasonPhrase);
    }

    private async Task<HttpResponseMessage> GetOrderInternal(string accountHashValue, string accessToken, string orderNumber)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{accountHashValue}/orders/{orderNumber}");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }
}