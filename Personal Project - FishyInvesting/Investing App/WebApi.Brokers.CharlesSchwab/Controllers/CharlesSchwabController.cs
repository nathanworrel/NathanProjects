using System.Globalization;
using System.Text.RegularExpressions;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.MarketTime;
using FishyLibrary.Models.Order;
using MakeTrade.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApi.Template.Models;
using WebApi.Template.Services;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApi.Template.Controllers;

[ApiController]
[Route("[controller]")]
public class CharlesSchwabController : ControllerBase
{
    private readonly ILogger<CharlesSchwabController> _logger;
    private readonly ICharlesSchwabService _charlesSchwabService;

    public CharlesSchwabController(ILogger<CharlesSchwabController> logger, ICharlesSchwabService service)
    {
        _logger = logger;
        _charlesSchwabService = service;
    }

    [HttpGet($"/SignIn")]
    public IActionResult SignIn(int userId)
    {
        string? accessToken = _charlesSchwabService.ManualSignIn(userId);
        if (accessToken == null)
        {
            _logger.LogError($"Unable to get access token for {userId}");
            return Unauthorized();
        }

        return Ok();
    }

    [HttpGet($"/AutomaticSignIn")]
    public IActionResult AutomaticSignIn(int userId)
    {
        string? accessToken = _charlesSchwabService.AutomaticSignIn(userId);
        if (accessToken == null)
        {
            _logger.LogError($"Unable to get access token for {userId}");
            return Unauthorized("Unable to get access token");
        }

        return Ok();
    }

    [HttpGet($"/EveryoneSignIn")]
    public IActionResult EveryoneSignIn()
    {
        try
        {
            _charlesSchwabService.EveryoneSignIn();
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpPost("/PlaceTrade")]
    public IActionResult PlaceTrade(FishyLibrary.Helpers.MakeTrade makeTrade, int accountId, bool dry)
    {
        try
        {
            _logger.LogInformation($"Recieved trade: {makeTrade} for account: {accountId}");

            Order order = new Order(makeTrade);

            HttpResponseMessage responseMessage = _charlesSchwabService.VerifySendMakeTrade(order, accountId).Result;
            if (!responseMessage.IsSuccessStatusCode)
            {
                _logger.LogError("{}", responseMessage);
                _logger.LogError($"Failed to validate: {order}");
                return BadRequest(responseMessage);
            }

            _logger.LogInformation(
                $"Verifying order successful for trade: {makeTrade.Side} {makeTrade.Quantity} {makeTrade.Product} at {makeTrade.Price} to account: {accountId}");

            if (dry)
            {
                _logger.LogInformation($"Dry run - trade not placed");
                return Ok();
            }

            _logger.LogInformation(
                $"Sending Order {makeTrade.Side} {makeTrade.Quantity} {makeTrade.Product} at {makeTrade.Price} to account: {accountId}");

            HttpResponseMessage response = _charlesSchwabService.SendMakeTrade(order, accountId).Result;
            _logger.LogInformation($"Sending Order Response: {JsonConvert.SerializeObject(response)}");
            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues("Location", out var values))
                {
                    var location = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(location))
                    {
                        var number = location.Split('/').Last();
                        _logger.LogInformation(
                            $"Extracted number: {number} for {makeTrade.Side} {makeTrade.Quantity} {makeTrade.Product} at {makeTrade.Price} to account: {accountId}");
                        return Ok(number);
                    }
                }

                return Ok();
            }

            return BadRequest(response);
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/Orders")]
    public IActionResult Orders(int accountId, string startTime = "12/15/2024")
    {
        _logger.LogInformation($"Getting orders for {accountId} after {startTime}");
        var time = DateTime.ParseExact(startTime, @"MM/dd/yyyy", CultureInfo.InvariantCulture);
        var result = _charlesSchwabService.GetOrders(accountId, time.Date).Result.Content.ReadAsStringAsync().Result;
        var orders = JsonSerializer.Deserialize<List<OrderData>>(result);
        return Ok(orders);
    }

    [HttpGet("/Order")]
    public IActionResult Order(int accountId, string orderNumber)
    {
        var result = _charlesSchwabService.GetOrder(accountId, orderNumber).Result.Content.ReadAsStringAsync().Result;
        var order = JsonSerializer.Deserialize<OrderData>(result);
        if (order == null)
        {
            return BadRequest();
        }

        return Ok(order);
    }

    [HttpGet("/CurrentMarketPrice")]
    public IActionResult CurrentMarketPrice(int accountId, string product)
    {
        try
        {
            HttpResponseMessage response = _charlesSchwabService.GetCurrentMarketPrice(accountId, product).Result;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                if (!result.IsNullOrEmpty())
                {
                    Match? lastPriceMatch = Regex.Matches(result, @"lastPrice.?.?([0-9.]*)").LastOrDefault();
                    if (lastPriceMatch is { Success: true })
                    {
                        return Ok(lastPriceMatch.Groups[1].Value);
                    }

                    throw new Exception("No lastPrice match");
                }
                throw new Exception("No RESULT");
            }

            return BadRequest(response);
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/AccountData")]
    public AccountInfo AccountData(int accountId)
    {
        _logger.LogInformation($"Getting account data from account {accountId}");

        string temp = _charlesSchwabService.GetAccountData(accountId).Result.Content.ReadAsStringAsync().Result;
        Regex shortRegex = new Regex(@"shortQuantity.?.?([0-9]*)");
        Match shortQuantitys = shortRegex.Match(temp);
        Regex averageRegex = new Regex(@"averagePrice.?.?([0-9.]*),");
        Match averageQuantities = averageRegex.Match(temp);
        Regex longRegex = new Regex(@"longQuantity.?.?([0-9]*)");
        Match longQuantities = longRegex.Match(temp);
        Regex symbolRegex = new Regex(@"symbol.?.?.?([A-Z]*)");
        Match symbolStuffs = symbolRegex.Match(temp);
        Dictionary<string, Standing> positions = new Dictionary<string, Standing>();
        while (shortQuantitys.Success)
        {
            // Console.WriteLine($"shortQuantity: {shortQuantitys}");
            positions[symbolStuffs.Groups[1].Value] = new Standing(symbolStuffs.Groups[1].Value,
                int.Parse(longQuantities.Groups[1].Value),
                int.Parse(shortQuantitys.Groups[1].Value), double.Parse(averageQuantities.Groups[1].Value));
            longQuantities = longQuantities.NextMatch();
            shortQuantitys = shortQuantitys.NextMatch();
            symbolStuffs = symbolStuffs.NextMatch();
            averageQuantities = averageQuantities.NextMatch();
        }

        Regex input = new Regex(@"cashBalance.?.?([0-9.]+)");
        Match availableFunds = input.Match(temp);
        Regex inputNumber = new Regex(@"accountNumber.?.?.?([0-9]*)");
        Match accountNumber = inputNumber.Match(temp);
        AccountInfo accountInfo = new AccountInfo(new Balance(double.Parse(availableFunds.Groups[1].Value)), positions);
        accountInfo.AccountNumber = accountNumber.Groups[1].Value;
        return accountInfo;
    }

    [HttpGet("/IsMarketOpen")]
    public MarketTime IsMarketOpen(int accountId)
    {
        return _charlesSchwabService.GetMarketTime(accountId).Result;
    }
}