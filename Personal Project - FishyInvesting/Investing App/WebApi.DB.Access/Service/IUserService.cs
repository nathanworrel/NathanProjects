using FishyLibrary.Models.User;

namespace WebApi.DB.Access.Service;

public interface IUserService
{
    int AddUser(UserPost user);
}