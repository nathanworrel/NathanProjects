using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FishyLibrary.Models.Trade;

public class TradeBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime TimePlaced { get; set; }
    public int QuantityPlaced { get; set; }
    public decimal PricePlaced { get; set; }
    public long OrderNumber { get; set; }
    public int Status { get; set; }
    public int Side { get; set; }
    public DateTime TimeModified { get; set; }
    public int QuantityFilled { get; set; }
    public decimal PriceFilled { get; set; }
    public decimal DesiredAllocation { get; set; }
    
    public TradeBase() {}

    public TradeBase(int id, 
        DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        long orderNumber,
        int status,
        int side,
        DateTime timeModified,
        int quantityFilled,
        decimal priceFilled,
        decimal desiredAllocation)
    {
        Id = id;
        TimePlaced = timePlaced;
        QuantityPlaced = quantityPlaced;
        PricePlaced = pricePlaced;
        OrderNumber = orderNumber;
        Status = status;
        Side = side;
        TimeModified = timeModified;
        QuantityFilled = quantityFilled;
        PriceFilled = priceFilled;
        DesiredAllocation = desiredAllocation;
    }
    
    public TradeBase(
        DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        long orderNumber,
        int status,
        int side,
        DateTime timeModified,
        int quantityFilled,
        decimal priceFilled,
        decimal desiredAllocation)
    {
        TimePlaced = timePlaced;
        QuantityPlaced = quantityPlaced;
        PricePlaced = pricePlaced;
        OrderNumber = orderNumber;
        Status = status;
        Side = side;
        TimeModified = timeModified;
        QuantityFilled = quantityFilled;
        PriceFilled = priceFilled;
        DesiredAllocation = desiredAllocation;
    }
    
    public TradeBase(DateTime timePlaced,
        int quantityPlaced,
        decimal pricePlaced,
        int side,
        decimal desiredAllocation)
    {
        Id = 0;
        TimePlaced = timePlaced;
        QuantityPlaced = quantityPlaced;
        PricePlaced = pricePlaced;
        OrderNumber = 0;
        Status = 0;
        Side = side;
        TimeModified = timePlaced;
        QuantityFilled = quantityPlaced;
        PriceFilled = pricePlaced;
        DesiredAllocation = desiredAllocation;
    }
}