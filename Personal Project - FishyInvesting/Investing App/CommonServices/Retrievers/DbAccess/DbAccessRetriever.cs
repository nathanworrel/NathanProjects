using System.Net.Http.Json;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;
using FishyLibrary.Utils;
using Microsoft.Extensions.Logging;

namespace CommonServices.Retrievers.DbAccess;

public class DbAccessRetriever : IDbAccessRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DbAccessRetriever> _logger;

    public DbAccessRetriever(IHttpClientFactory httpClient, string environmentName, ILogger<DbAccessRetriever> logger)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri(ApiBaseUrls.BaseUrl(environmentName, Api.DbAccess));
    }

    public async Task<bool> AddParameters(Parameters parameters)
    {
        HttpResponseMessage resposne = await _httpClient.PostAsync("Parameters", JsonContent.Create(parameters));
        if (!resposne.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Unable to add parameters");
        }

        return true;
    }
}