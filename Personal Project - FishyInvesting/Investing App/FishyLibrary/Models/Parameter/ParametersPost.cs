namespace FishyLibrary.Models.Parameters;

public class ParametersPost : ParametersBase
{
    public int StrategyTypeId { get; set; }
    
    public ParametersPost() {}

    public ParametersPost(int id, Dictionary<string, double?> parameters, int strategyTypeid) 
        : base(id, parameters)
    {
        StrategyTypeId = strategyTypeid;
    }
}