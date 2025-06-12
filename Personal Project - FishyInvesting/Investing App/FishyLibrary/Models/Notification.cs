namespace FishyLibrary.Models;

public class Notification : INotification
{
    public string Message { get; set; }
    
    public Int64 ChatId { get; set; }
}