// using System.Net;
// using System.Net.Http.Json;
// using FishyLibrary.Helpers;
// using FishyLibrary.Models;
// using FishyLibrary.Utils;
// using MakeTrade.Services;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging.Abstractions;
// using Moq;
// using RichardSzalay.MockHttp;
// using WebApi.GetData.Context;
//
// namespace UnitTests.WebApi.MakeTrade;
//
// public class MakeTradeServiceTest
// {
//     private readonly string devBaseUrl = "https://localhost:7226/";
//     private readonly string stagingBaseUrl = "http://charles-schwab:8080/";
//     private readonly string accountDataUrl = "AccountData";
//     private readonly string makeTradeUrl = "PlaceTrade";
//
//     private static Mock<IWebHostEnvironment> GetMockedWebHostEnvironment(bool isDevelopment)
//     {
//         var mockedWebHostEnvironment = new Mock<IWebHostEnvironment>();
//         mockedWebHostEnvironment
//             .Setup(x => x.EnvironmentName)
//             .Returns(isDevelopment ? "Development" : "Production");
//         return mockedWebHostEnvironment;
//     }
//
//     private static Mock<IHttpClientFactory> GetMockedHttpClientFactory()
//     {
//         Mock<IHttpClientFactory> factory = new Mock<IHttpClientFactory>();
//         factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
//         return factory;
//     }
//
//     [Fact]
//     public async void MakeTradeService_DevelopmentBaseAddress()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         mockHttp
//             .When($"{devBaseUrl}{accountDataUrl}?userId={userId}")
//             .Respond(_ =>
//             {
//                 HttpResponseMessage reply = new HttpResponseMessage(HttpStatusCode.OK);
//                 reply.Content = JsonContent.Create(new AccountInfo());
//                 return reply;
//             });
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(true);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         AccountInfo? accountInfo = await service.GetAccountInfo(userId);
//
//         // Assert
//         Assert.NotNull(accountInfo);
//     }
//
//     [Fact]
//     public async void MakeTradeService_StagingBaseAddress()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         mockHttp
//             .When($"{stagingBaseUrl}{accountDataUrl}?userId={userId}")
//             .Respond(_ =>
//             {
//                 HttpResponseMessage reply = new HttpResponseMessage(HttpStatusCode.OK);
//                 reply.Content = JsonContent.Create(new AccountInfo());
//                 return reply;
//             });
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         AccountInfo? accountInfo = await service.GetAccountInfo(userId);
//
//         // Asserts
//         Assert.NotNull(accountInfo);
//     }
//
//     [Fact]
//     public async void MakeTradeService_IncorrectBaseAddress()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         var request = mockHttp
//             .When($"{devBaseUrl}{accountDataUrl}?userId={userId}")
//             .Respond(_ =>
//             {
//                 HttpResponseMessage reply = new HttpResponseMessage(HttpStatusCode.OK);
//                 reply.Content = JsonContent.Create(new AccountInfo());
//                 return reply;
//             });
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make call
//         AccountInfo? accountInfo = await service.GetAccountInfo(userId);
//
//         // Asserts
//         Assert.Null(accountInfo);
//         Assert.Equal(0, mockHttp.GetMatchCount(request));
//     }
//
//     [Fact]
//     public async void MakeTradeService_GetAccountInfo_Correct()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         Balance balance = new Balance(10);
//         Position position1 = new Position("QQQ", 10, 10, 10);
//         Position position2 = new Position("TQQQ", 100, 100, 100);
//         mockHttp
//             .When($"{stagingBaseUrl}{accountDataUrl}?userId={userId}")
//             .Respond(_ =>
//             {
//                 HttpResponseMessage reply = new HttpResponseMessage(HttpStatusCode.OK);
//                 AccountInfo accountInfo = new AccountInfo(balance);
//                 accountInfo.Positions["QQQ"] = position1;
//                 accountInfo.Positions["TQQQ"] = position2;
//                 reply.Content = JsonContent.Create(accountInfo);
//                 return reply;
//             });
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         AccountInfo? accountInfo = await service.GetAccountInfo(userId);
//
//         // Asserts
//         Assert.NotNull(accountInfo);
//         Assert.Equal(balance, accountInfo.Balance);
//         Assert.Equal(position1, accountInfo.Positions["QQQ"]);
//         Assert.Equal(position2, accountInfo.Positions["TQQQ"]);
//     }
//
//     [Fact]
//     public async void MakeTradeService_GetAccountInfo_NoAccount()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         mockHttp
//             .When($"{stagingBaseUrl}{accountDataUrl}?userId={userId}")
//             .Respond(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make call
//         AccountInfo? accountInfo = await service.GetAccountInfo(userId);
//
//         // Asserts
//         Assert.Null(accountInfo);
//     }
//
//     [Fact]
//     public async void MakeTradeService_SendTrade()
//     {
//         // Setup
//         using var mockHttp = new MockHttpMessageHandler();
//         int userId = 1;
//         bool dry = true;
//         mockHttp
//             .When($"{stagingBaseUrl}{makeTradeUrl}?accountNumber=0&userId={userId}&dry={dry}")
//             .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK));
//         MockHttpClientFactory factory = new MockHttpClientFactory(mockHttp);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         MakeTradeService service = new MakeTradeService(env.Object, factory, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         HttpResponseMessage response = await service.SendTrade(null, "0", userId, dry);
//
//         // Asserts
//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//     }
//
//     [Fact]
//     public void MakeTradeService_GenerateTrade_Buy()
//     {
//         // Setup
//         double currentPrice = 100;
//         AccountInfo accountInfo = new AccountInfo(new Balance(100));
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         Mock<IHttpClientFactory> factory = GetMockedHttpClientFactory();
//         MakeTradeService service =
//             new MakeTradeService(env.Object, factory.Object, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         FishyLibrary.Helpers.MakeTrade? trade = service.GenerateTrade(1, "QQQ", currentPrice, accountInfo);
//
//         // Asserts
//         Assert.NotNull(trade);
//         Assert.Equal(Side.BUY, trade.Side);
//         Assert.Equal(1, trade.Quantity);
//         Assert.Equal(currentPrice, trade.Price);
//     }
//
//     [Fact]
//     public void MakeTradeService_GenerateTrade_BuyMultiple()
//     {
//         // Setup
//         double currentPrice = 10;
//         AccountInfo accountInfo = new AccountInfo(new Balance(100));
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         Mock<IHttpClientFactory> factory = GetMockedHttpClientFactory();
//         MakeTradeService service =
//             new MakeTradeService(env.Object, factory.Object, NullLogger<MakeTradeService>.Instance);
//
//         // Make Call
//         FishyLibrary.Helpers.MakeTrade? trade = service.GenerateTrade(1, "QQQ", currentPrice, accountInfo);
//
//         // Asserts
//         Assert.NotNull(trade);
//         Assert.Equal(Side.BUY, trade.Side);
//         Assert.Equal(10, trade.Quantity);
//         Assert.Equal(currentPrice, trade.Price);
//     }
//
//     [Fact]
//     public void MakeTradeService_GenerateTrade_BuyMultipleRemainder()
//     {
//         // Setup
//         double currentPrice = 15;
//         AccountInfo accountInfo = new AccountInfo(new Balance(100));
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         Mock<IHttpClientFactory> factory = GetMockedHttpClientFactory();
//         MakeTradeService service =
//             new MakeTradeService(env.Object, factory.Object, NullLogger<MakeTradeService>.Instance);
//
//         // Make Calls
//         FishyLibrary.Helpers.MakeTrade? trade = service.GenerateTrade(1, "QQQ", currentPrice, accountInfo);
//
//         // Asserts
//         Assert.NotNull(trade);
//         Assert.Equal(Side.BUY, trade.Side);
//         Assert.Equal(6, trade.Quantity);
//         Assert.Equal(currentPrice, trade.Price);
//     }
//
//     [Fact]
//     public void MakeTradeService_GenerateTrade_Sell()
//     {
//         // Setup
//         double currentPrice = 100;
//         AccountInfo accountInfo = new AccountInfo(new Balance(100));
//         accountInfo.Positions["QQQ"] = new Position("QQQ", 5, 0, 10);
//         Mock<IWebHostEnvironment> env = GetMockedWebHostEnvironment(false);
//         Mock<IHttpClientFactory> factory = GetMockedHttpClientFactory();
//         MakeTradeService service =
//             new MakeTradeService(env.Object, factory.Object, NullLogger<MakeTradeService>.Instance);
//
//         // Make Calls
//         FishyLibrary.Helpers.MakeTrade? trade = service.GenerateTrade(0, "QQQ", currentPrice, accountInfo);
//
//         // Asserts
//         Assert.NotNull(trade);
//         Assert.Equal(Side.SELL, trade.Side);
//         Assert.Equal(5, trade.Quantity);
//         Assert.Equal(currentPrice, trade.Price);
//     }
// }
// // https://learn.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api
// // https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#a-simple-test-using-xunits-fixtures