using FishyLibrary.Models;

namespace FishyLibrary.Helpers;

/*
 * Record holding all the info pertaining to the trade we want to make.
 *
 * Used to send trades between MakeTrade and CharlesSchwab.
 */
public record MakeTrade(string Product, 
    int Quantity, 
    Side Side, 
    double Price, 
    double PercentageChange, 
    DateTime DateTime);