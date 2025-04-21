using System.Text.Json.Serialization;

namespace FishyLibrary.Models.MarketTime;

public class EquityData
{
    [JsonPropertyName("EQ")]
    public EquityDetails? EQ { get; set; }

    [JsonPropertyName("equity")]
    public EquityDetails? Equity { get; set; }
}
