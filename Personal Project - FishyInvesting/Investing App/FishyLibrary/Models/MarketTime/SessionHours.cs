using System.Text.Json.Serialization;

namespace FishyLibrary.Models.MarketTime;

public class SessionHours
{
    [JsonPropertyName("preMarket")]
    public List<MarketSession> PreMarket { get; set; }

    [JsonPropertyName("regularMarket")]
    public List<MarketSession> RegularMarket { get; set; }

    [JsonPropertyName("postMarket")]
    public List<MarketSession> PostMarket { get; set; }
}