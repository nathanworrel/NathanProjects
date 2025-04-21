namespace MakeTrade.Models;

/*
 * Record used to store any information on the account balance. Record type is used so it's immutable.
 * The balance should not change when deciding to buy or sell
 */
public record Balance(double CashBalance);