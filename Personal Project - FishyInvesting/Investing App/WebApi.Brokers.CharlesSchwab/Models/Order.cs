using FishyLibrary.Helpers;
using FishyLibrary.Models;

namespace WebApi.Template.Models;

public class Order
{
    public string orderType;
    public double price;
    public string session;
    public string duration;
    public string orderStrategyType;
    public List<Leg> orderLegCollection;

    public Order(FishyLibrary.Helpers.MakeTrade makeTrade)
    {
        price = makeTrade.Price > 1
            ? (makeTrade.Side == Side.BUY ? Math.Floor(makeTrade.Price * 100) / 100 : Math.Ceiling(makeTrade.Price * 100) / 100)
            : makeTrade.Price;
        orderType = "LIMIT";
        session = "NORMAL";
        duration = "DAY";
        orderStrategyType = "SINGLE";
        orderLegCollection = new List<Leg> { new Leg(makeTrade) };
    }

    public class Leg
    {
        public string instruction;
        public int quantity;
        public Instrument instrument;

        public Leg(FishyLibrary.Helpers.MakeTrade makeTrade)
        {
            instruction = makeTrade.Side.ToString().ToUpper(); // BUY/SELL
            quantity = makeTrade.Quantity;
            instrument = new Instrument(makeTrade.Product);
        }

        public class Instrument
        {
            public string symbol;
            public string assetType;

            public Instrument(string product)
            {
                assetType = "EQUITY";
                symbol = product;
            }
        }
    }
}