using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;

namespace CommonServices.Retrievers.StrategyService;

public class StrategyServiceRetriever : IStrategyServiceRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StrategyServiceRetriever> _logger;

    public StrategyServiceRetriever(IHttpClientFactory httpClient, string environmentName,
        ILogger<StrategyServiceRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.StrategyService));
    }

    public void SendToStrategyService(TimeSpan timeSpan, int strategyId)
    {
        SendToStrategyServiceInternal(timeSpan, strategyId);
    }

    private void SendToStrategyServiceInternal(TimeSpan runTime, int strategyId)
    {
        _logger.LogInformation($"Sending strategy {strategyId} at {runTime.ToString()}");
        HttpResponseMessage response = _httpClient.PutAsync(
            $"StrategyService/StrategyRun?strategyId={strategyId}&runTime={runTime}",
            null).Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Sending strategy {strategyId} failed with status code {response.StatusCode}");
        }
    }
}