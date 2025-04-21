namespace FishyLibrary.Models.User;

public class UserBase
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAutomatic { get; set; }

    public UserBase() {}
    
    public UserBase(int id, string username, string password, bool isAutomatic)
    {
        Id = id;
        Username = username;
        Password = password;
        IsAutomatic = isAutomatic;
    }
}