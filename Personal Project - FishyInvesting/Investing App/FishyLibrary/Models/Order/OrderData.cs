using System.Text.Json.Serialization;

namespace FishyLibrary.Models.Order;

public class OrderData
{
    [JsonPropertyName("session")]
    public string Session { get; set; }
    [JsonPropertyName("duration")]
    public string Duration { get; set; }
    [JsonPropertyName("orderType")]
    public string OrderType { get; set; }
    [JsonPropertyName("complexOrderStrategyType")]
    public string ComplexOrderStrategyType { get; set; }
    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }
    [JsonPropertyName("filledQuantity")]
    public double FilledQuantity { get; set; }
    [JsonPropertyName("remainingQuantity")]
    public double RemainingQuantity { get; set; }
    [JsonPropertyName("requestedDestination")]
    public string RequestedDestination { get; set; }
    [JsonPropertyName("destinationLinkName")]
    public string DestinationLinkName { get; set; }
    [JsonPropertyName("orderLegCollection")]
    public List<OrderLeg> OrderLegCollection { get; set; }
    [JsonPropertyName("orderStrategyType")]
    public string OrderStrategyType { get; set; }
    [JsonPropertyName("orderId")]
    public long OrderId { get; set; }
    [JsonPropertyName("cancelable")]
    public bool Cancelable { get; set; }
    [JsonPropertyName("editable")]
    public bool Editable { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } 
    [JsonPropertyName("enteredTime")]
    public string EnteredTime { get; set; }
    [JsonPropertyName("closeTime")]
    public string CloseTime { get; set; }
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    [JsonPropertyName("accountNumber")]
    public int AccountNumber { get; set; }
    [JsonPropertyName("orderActivityCollection")]
    public List<OrderActivity> OrderActivityCollection { get; set; }
    [JsonPropertyName("statusDescription")]
    public string StatusDescription { get; set; }
}