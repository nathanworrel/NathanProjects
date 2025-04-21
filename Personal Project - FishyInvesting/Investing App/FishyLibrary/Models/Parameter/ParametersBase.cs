namespace FishyLibrary.Models.Parameters;

public class ParametersBase
{
    public int Id { get; set; }
    
    public Dictionary<string, double?> Params { get; set; }
    
    public ParametersBase() {}

    public ParametersBase(int id, Dictionary<string, double?> parameters)
    {
        Id = id;
        Params = parameters;
    }
}