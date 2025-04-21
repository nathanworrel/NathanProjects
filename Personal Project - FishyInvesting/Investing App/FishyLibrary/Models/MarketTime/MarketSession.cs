using System.Text.Json.Serialization;

namespace FishyLibrary.Models.MarketTime;

public class MarketSession
{
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    [JsonPropertyName("end")]
    public DateTime End { get; set; }
}