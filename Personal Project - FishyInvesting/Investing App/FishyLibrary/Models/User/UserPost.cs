namespace FishyLibrary.Models.User;

public class UserPost : UserBase
{
    public UserPost() {}

    public UserPost(string username, string password, bool isAutomatic) : base(0, username, password, isAutomatic)
    {
        
    }
}