using AutoMapper;
using FishyLibrary.Models.Account;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB.Access.Contexts;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("Account")]
public class AccountController : Controller
{
    private ILogger<AccountController> _logger;
    private IAccountService _accountService;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [HttpGet("all")]
    public ActionResult<List<AccountGet>> Get()
    {
        _logger.LogInformation("Get all accounts");
        return _accountService.GetAllAccounts();
    }
}