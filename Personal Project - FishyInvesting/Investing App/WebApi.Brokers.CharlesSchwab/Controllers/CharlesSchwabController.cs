using FishyLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Template.Services;

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
    public IActionResult SignIn(int clientId)
    {
        string? accessToken = _charlesSchwabService.ManualSignIn(clientId);
        if (accessToken == null)
        {
            _logger.LogError("Unable to get access token for {clientId}", clientId);
            return Unauthorized();
        }

        return Ok();
    }

    [HttpGet($"/AutomaticSignIn")]
    public IActionResult AutomaticSignIn(int clientId)
    {
        string? accessToken = _charlesSchwabService.AutomaticSignIn(clientId);
        if (accessToken == null)
        {
            _logger.LogError("Unable to get access token for {clientId}", clientId);
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
            _logger.LogError("Unable to log everyone in with error {error}", e);
            throw;
        }
    }

    [HttpPost("/PlaceTrade")]
    public IActionResult PlaceTrade(FishyLibrary.Helpers.MakeTrade makeTrade, int accountId, bool dry)
    {
        try
        {
            GenericResponse responseMessage = _charlesSchwabService.PlaceTrade(makeTrade, accountId, dry);
            if (responseMessage.isSuccess)
            {
                return Ok(responseMessage.Message);
            }

            return BadRequest(responseMessage.Message);
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
        _logger.LogInformation("Getting orders for {accountId} after {startTime}", accountId, startTime);
        try
        {
            return Ok(_charlesSchwabService.GetOrders(accountId, startTime));
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/Order")]
    public IActionResult Order(int accountId, string orderNumber)
    {
        try
        {
            return Ok(_charlesSchwabService.GetOrder(accountId, orderNumber));
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/CurrentMarketPrice")]
    public IActionResult CurrentMarketPrice(int accountId, string product)
    {
        try
        {
            return Ok(_charlesSchwabService.GetCurrentMarketPrice(accountId, product));
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/AccountData")]
    public IActionResult AccountData(int accountId)
    {
        _logger.LogInformation("Getting account data from account {accountId}", accountId);
        try
        {
            return Ok(_charlesSchwabService.GetAccountData(accountId));
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }

    [HttpGet("/IsMarketOpen")]
    public IActionResult IsMarketOpen(int accountId)
    {
        try
        {
            return Ok(_charlesSchwabService.GetMarketTime(accountId));
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            throw;
        }
    }
}