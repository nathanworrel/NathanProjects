using AutoMapper;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Strategy;
using FishyLibrary.Models.Client;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class ClientService : IClientService
{
    private readonly ILogger<ClientService> _logger;
    private DBAccessContext _dbAccessContext;
    private IMapper _mapper;

    public ClientService(DBAccessContext dbAccessContext, IMapper mapper, ILogger<ClientService> logger)
    {
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
        _logger = logger;
    }

    public Client? Find(int id)
    {
        try
        {
            return _dbAccessContext.Users
                .FirstOrDefault(u => u.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public List<Client> GetAll()
    {
        try
        {
            return _dbAccessContext.Users.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new List<Client>();
        }
    }

    public int AddUser(ClientPost client)
    {
        try
        {
            Client clientEntity = _mapper.Map<Client>(client);
            clientEntity.TelegramChatId = "1";
            _dbAccessContext.Users.Add(clientEntity);
            _dbAccessContext.SaveChanges();
            return clientEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public List<Account>? GetAccounts(int userId)
    {
        try
        {
            var user = _dbAccessContext.Users
                .Include(u => u.Accounts)
                .FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }
            return user.Accounts?.ToList();
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return null;
        }
    }
}