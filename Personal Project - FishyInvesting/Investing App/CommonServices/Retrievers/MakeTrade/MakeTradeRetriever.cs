using FishyLibrary.Models;
using FishyLibrary.Models.Trade;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;

namespace CommonServices.Retrievers.MakeTrade;

public class MakeTradeRetriever : IMakeTradeRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MakeTradeRetriever> _logger;

    public MakeTradeRetriever(IHttpClientFactory httpClient, string environmentName, ILogger<MakeTradeRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.MakeTrade));
    }

    public void Trade(double desiredPosition, double price, int strategyId, bool dry)
    {
        TradeInternal(desiredPosition, price, strategyId, dry);
    }

    private void TradeInternal(double desiredPosition, double price, int strategyId, bool dry)
    {
        HttpResponseMessage response = _httpClient.PostAsync(
                $"MakeTrade?desiredPosition={desiredPosition}&strategyId={strategyId}&currentPrice={price}&dry={dry}",
                null)
            .Result;
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"MakeTrade returned status code {response.StatusCode} and error message {response.ReasonPhrase}");
        }

        var result = response.Content.ReadAsStringAsync().Result;
        if (!result.Contains("null"))
        {
            _logger.LogInformation($"Trade response: {result}");
        }
    }
}