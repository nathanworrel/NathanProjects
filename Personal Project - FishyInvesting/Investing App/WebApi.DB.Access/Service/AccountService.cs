using AutoMapper;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.Strategy;
using Microsoft.EntityFrameworkCore;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private DBAccessContext _dbAccessContext;
    private IMapper _mapper;

    public AccountService(ILogger<AccountService> logger, DBAccessContext dbAccessContext, IMapper mapper)
    {
        _logger = logger;
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
    }

    public List<AccountGet> GetAllAccounts()
    {
        return _dbAccessContext.Accounts.Select(a => _mapper.Map<AccountGet>(a)).ToList();
    }

    public int AddAccount(AccountPost account)
    {
        try
        {
            Account accountEntity = _mapper.Map<Account>(account);
            _dbAccessContext.Accounts.Add(accountEntity);
            _dbAccessContext.SaveChanges();
            return accountEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }

    public Account? Find(int id)
    {
        return _dbAccessContext.Accounts.FirstOrDefault(a => a.Id == id);
    }

    public AccountPost? Delete(Account account)
    {
        try
        {
            _dbAccessContext.Accounts.Remove(account);
            _dbAccessContext.SaveChanges();
            return _mapper.Map<AccountPost>(account);
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return null;
        }
    }

    public List<Strategy>? GetStrategiesForAccount(int accountId)
    {
        var account = _dbAccessContext.Accounts
            .Include(x => x.Strategies)
            .FirstOrDefault(a => a.Id == accountId);
        if (account == null)
        {
            return null;
        }
        return account.Strategies?.ToList();
    }
}