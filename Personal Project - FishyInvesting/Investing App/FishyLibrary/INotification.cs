namespace FishyLibrary;

public interface INotification
{
    public string Message { get; set; }
    
    public Int64 ChatId { get; set; }
}