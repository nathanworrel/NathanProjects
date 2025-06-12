namespace WebApi.Template.Models;

public class SecuritiesAccount
{
    public string accountNumber { get; set; }
    public List<Position> positions { get; set; }
    public CurrentBalances currentBalances { get; set; }
}