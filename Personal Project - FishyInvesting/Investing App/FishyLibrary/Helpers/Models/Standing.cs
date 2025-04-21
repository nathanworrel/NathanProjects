namespace FishyLibrary.Models;

/*
 * Record holding information pertaining to a position.
 *
 * Used to save data from Charles Schwab api before making a trade.
 */
public record Standing(
    string Product, 
    int LongQuantity, 
    int ShortQuantity, 
    double AveragePrice);