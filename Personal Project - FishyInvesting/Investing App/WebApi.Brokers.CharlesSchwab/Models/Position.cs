namespace WebApi.Template.Models;

public class Position
{
    public double shortQuantity { get; set; }
    public double averagePrice { get; set; }
    public double longQuantity { get; set; }
    public double settledLongQuantity { get; set; }
    public double settledShortQuantity { get; set; }
    public Instrument instrument { get; set; }
}