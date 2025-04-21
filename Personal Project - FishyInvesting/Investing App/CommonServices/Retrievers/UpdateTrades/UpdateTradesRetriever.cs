using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;

namespace CommonServices.Retrievers.UpdateTrades;

public class UpdateTradesRetriever : IUpdateTradesRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UpdateTradesRetriever> _logger;

    public UpdateTradesRetriever(IHttpClientFactory httpClient, string environmentName,
        ILogger<UpdateTradesRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.UpdateTrades));
    }

    public void CallUpdateTrades()
    {
        CallUpdateTradesInternal().Wait();
    }

    private async Task CallUpdateTradesInternal()
    {
        HttpResponseMessage response =
            await _httpClient.PatchAsync($"UpdateTrades", null);
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new Exception($"Failed to call updateTrades: {response.ReasonPhrase}");
    }
}