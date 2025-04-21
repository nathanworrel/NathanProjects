using FishyLibrary.Models;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;

namespace CommonServices.Retrievers.Scheduler;

public class SchedulerRetriever : ISchedulerRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SchedulerRetriever> _logger;

    public SchedulerRetriever(IHttpClientFactory httpClient, string environmentName, ILogger<SchedulerRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.Scheduler));
    }

    public void CallScheduler(TimeSpan time)
    {
        CallSchedulerInternal(time).Wait();
    }

    private async Task CallSchedulerInternal(TimeSpan time)
    {
        HttpResponseMessage response =
            await _httpClient.PostAsync($"Scheduler/start?runTime={time}",
                null);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to call scheduler: {time} with reason {response.ReasonPhrase}");
        }
    }
}