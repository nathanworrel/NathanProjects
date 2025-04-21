namespace FishyLibrary.Models.Parameters;

public class ParametersGet
{
    public int Id { get; set; }
    
    public StrategyTypeGet StrategyType { get; set; }
    
    public Dictionary<string, double?> Params { get; set; }
}