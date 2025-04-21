namespace FishyLibrary.Models.StrategyRuntime;

public class StrategyRuntimeString
{
    public int Id { get; set; }
    public string Runtime { get; set; }

    public StrategyRuntimeString(int id, string runtime)
    {
        Id = id;
        Runtime = runtime;
    }
}