using System.Text.Json.Serialization;

namespace FishyLibrary.Models.Order;

public class ExecutionLeg
{
    [JsonPropertyName("legId")]
    public int LegId { get; set; }
    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }
    [JsonPropertyName("mismarkedQuantity")]
    public double MismarkedQuantity { get; set; }
    [JsonPropertyName("price")]
    public double Price { get; set; }
    [JsonPropertyName("time")]
    public string Time { get; set; }
    [JsonPropertyName("instrumentId")]
    public int InstrumentId { get; set; }
}