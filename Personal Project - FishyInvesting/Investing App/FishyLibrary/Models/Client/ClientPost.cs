namespace FishyLibrary.Models.Client;

public class ClientPost : ClientBase
{
    public ClientPost() {}

    public ClientPost(string username, string password, bool isAutomatic) : base(0, username, password, isAutomatic)
    {
        
    }
}