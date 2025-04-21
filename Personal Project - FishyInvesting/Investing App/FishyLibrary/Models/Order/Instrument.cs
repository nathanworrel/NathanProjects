using System.Text.Json.Serialization;

namespace FishyLibrary.Models.Order;

public class Instrument
{
    [JsonPropertyName("assetType")]
    public string AssetType { get; set; }
    [JsonPropertyName("cusip")]
    public string Cusip { get; set; }
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("instrumentId")]
    public int InstrumentId { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
}