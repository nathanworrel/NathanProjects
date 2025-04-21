using System.Text.Json.Serialization;

namespace FishyLibrary.Models.Order;

public class OrderActivity
{
    [JsonPropertyName("activityType")]
    public string ActivityType { get; set; }
    [JsonPropertyName("activityId")]
    public long ActivityId { get; set; }
    [JsonPropertyName("executionType")]
    public string ExecutionType { get; set; }
    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }
    [JsonPropertyName("orderRemainingQuantity")]
    public double OrderRemainingQuantity { get; set; }
    [JsonPropertyName("executionLegs")]
    public List<ExecutionLeg> ExecutionLegs { get; set; }
}