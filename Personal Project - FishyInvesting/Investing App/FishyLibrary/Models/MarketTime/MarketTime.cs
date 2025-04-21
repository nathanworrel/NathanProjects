using System.Text.Json.Serialization;

namespace FishyLibrary.Models.MarketTime;

public class MarketTime
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("isOpen")]
    public bool IsOpen { get; set; }

    [JsonPropertyName("open")]
    public TimeSpan Open { get; set; }

    [JsonPropertyName("close")]
    public TimeSpan Close { get; set; }
}