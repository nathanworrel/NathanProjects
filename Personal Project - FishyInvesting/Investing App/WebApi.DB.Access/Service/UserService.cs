using AutoMapper;
using FishyLibrary.Models.User;
using WebApi.DB.Access.Contexts;

namespace WebApi.DB.Access.Service;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private DBAccessContext _dbAccessContext;
    private IMapper _mapper;

    public UserService(DBAccessContext dbAccessContext, IMapper mapper, ILogger<UserService> logger)
    {
        _dbAccessContext = dbAccessContext;
        _mapper = mapper;
        _logger = logger;
    }

    public int AddUser(UserPost user)
    {
        try
        {
            User userEntity = _mapper.Map<User>(user);
            _dbAccessContext.Users.Add(userEntity);
            _dbAccessContext.SaveChanges();
            return userEntity.Id;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return 0;
        }
    }
}