using System.Text.Json.Serialization;

namespace FishyLibrary.Models.Order;

public class OrderLeg
{
    [JsonPropertyName("orderLegType")]
    public string OrderLegType { get; set; }
    [JsonPropertyName("legId")]
    public int LegId { get; set; }
    [JsonPropertyName("instrument")]
    public Instrument Instrument { get; set; }
    [JsonPropertyName("instruction")]
    public string Instruction { get; set; }
    [JsonPropertyName("positionEffect")]
    public string PositionEffect { get; set; }
    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }
}