namespace FishyLibrary.Models;

public class Return
{
    public DateTime Date { get; set; }
    public decimal Returns { get; set; }
    
    public decimal Value { get; set; }
    
    public Return(DateTime Date, decimal Returns)
    {
        this.Date = Date;
        this.Returns = Returns;
    }
    
    public Return(DateTime Date, decimal Returns, decimal value)
    {
        this.Date = Date;
        this.Returns = Returns;
        this.Value = value;
    }
}