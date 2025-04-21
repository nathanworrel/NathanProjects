namespace FishyLibrary.Models;

public record StockData(
    string Product,
    DateTime Time,
    decimal Price);