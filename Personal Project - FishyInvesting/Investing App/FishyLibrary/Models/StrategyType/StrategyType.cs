namespace FishyLibrary.Models.StrategyType;

public class StrategyType : StrategyTypeBase
{
    public ICollection<Parameters.Parameters> Parameters { get; set; }

    public StrategyType() : base()
    {
        Parameters = new List<Parameters.Parameters>();
    }

    public StrategyType(StrategyType other) : base(other)
    {
        Parameters = other.Parameters;
    }

    public StrategyType(int id, string name) : base(id, name) { }
}