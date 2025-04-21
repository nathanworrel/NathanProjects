using System.Text.Json.Serialization;

namespace FishyLibrary.Models.MarketTime;

public class EquityDetails
{
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("marketType")]
    public string MarketType { get; set; }

    [JsonPropertyName("product")]
    public string Product { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; }

    [JsonPropertyName("isOpen")]
    public bool IsOpen { get; set; }

    [JsonPropertyName("sessionHours")]
    public SessionHours SessionHours { get; set; }
}