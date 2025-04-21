using FishyLibrary.Utils;
using RichardSzalay.MockHttp;

namespace FishyLibrary.Extensions;

public static class MockHttpMessageHandlerExtensions
{
    public static IHttpClientFactory ToHttpClientFactory(this MockHttpMessageHandler mockHttpMessageHandler)
    {
        return new MockHttpClientFactory(mockHttpMessageHandler);
    }
}