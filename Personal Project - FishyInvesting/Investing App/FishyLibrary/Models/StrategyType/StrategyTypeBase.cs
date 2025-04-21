namespace FishyLibrary.Models;

public class StrategyTypeBase
{
    public int Id { get; set; }
    /*
     * Name of the strategy Ex: "MacD"
     */
    public string Name { get; set; }

    public StrategyTypeBase() {}

    public StrategyTypeBase(StrategyTypeBase other)
    {
        Id = other.Id;
        Name = other.Name;
    }

    public StrategyTypeBase(int id, string name)
    {
        Id = id;
        Name = name;
    }
}