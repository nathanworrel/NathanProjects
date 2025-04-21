using FishyLibrary.Models.Strategy;

namespace FishyLibrary.Models.Trade;

public class TradeGet : TradeBase
{
    public StrategyGet Strategy { get; set; }
}