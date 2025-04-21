using System.Text.Json.Serialization;
using FishyLibrary.Models.MarketTime;

public class EquityResponse
{
    [JsonPropertyName("equity")]
    public EquityData Equity { get; set; }
}
