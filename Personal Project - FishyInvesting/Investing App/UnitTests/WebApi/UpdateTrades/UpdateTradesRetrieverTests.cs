using System.Net;
using System.Text.Json;
using CommonServices.Retrievers.CharlesSchwab;
using FishyLibrary.Models;
using FishyLibrary.Models.Order;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WebApi.UpdateTrades.Services;

namespace UnitTests.WebApi.UpdateTrades;

public class UpdateTradesRetrieverTests
{
    private readonly Mock<ILogger<CharlesSchwabRetriever>> _mockLogger;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _mockHttpClient;
    private readonly ICharlesSchwabRetriever _service;

    public UpdateTradesRetrieverTests()
    {
        _mockLogger = new Mock<ILogger<CharlesSchwabRetriever>>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();
        _mockEnvironment.Setup(env => env.EnvironmentName).Returns("Development");

        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://mock-api-url/")
        };

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(_mockHttpClient);

        _service = new CharlesSchwabRetriever(_mockHttpClientFactory.Object, _mockEnvironment.Object.EnvironmentName, _mockLogger.Object);
    }

    [Fact]
    public void GetOrder_SuccessfulResponse_ReturnsDeserializedOrderData()
    {
        // Arrange
        var accountId = 123;
        var orderId = 456;
        var requestUri = $"Order?accountId={accountId}&orderNumber={orderId}";

        var mockOrderData = new OrderData
        {
            OrderId = orderId,
            AccountNumber = accountId,
            Status = "Filled",
            Quantity = 10
        };

        var responseContent = JsonSerializer.Serialize(mockOrderData);

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseContent)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri.ToString() == $"{_mockHttpClient.BaseAddress}{requestUri}"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        // Act
        var result = _service.GetOrder(accountId, orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(accountId, result.AccountNumber);
        Assert.Equal("Filled", result.Status);
        Assert.Equal(10, result.Quantity);
    }
}