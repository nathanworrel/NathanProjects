using AutoMapper;
using FishyLibrary.Models.Client;
using Microsoft.AspNetCore.Mvc;
using WebApi.DB.Access.Service;

namespace WebApi.DB.Access.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController : ControllerBase
{
    private ILogger<ClientController> _logger;
    private IClientService _clientService;
    private IMapper _mapper;

    public ClientController(ILogger<ClientController> logger, IClientService clientService, IMapper mapper)
    {
        _logger = logger;
        _clientService = clientService;
        _mapper = mapper;
    }

    [HttpGet("all")]
    public ActionResult<List<ClientGet>> GetAll()
    {
        return _clientService.GetAll().Select(user => _mapper.Map<ClientGet>(user)).ToList();
    }
}