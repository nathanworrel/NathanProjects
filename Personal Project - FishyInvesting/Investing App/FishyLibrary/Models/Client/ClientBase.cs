namespace FishyLibrary.Models.Client;

public class ClientBase
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsAutomatic { get; set; }
    public string TelegramChatId { get; set; }

    public ClientBase() {}
    
    public ClientBase(int id, string username, string password, bool isAutomatic)
    {
        Id = id;
        Username = username;
        Password = password;
        IsAutomatic = isAutomatic;
    }
}