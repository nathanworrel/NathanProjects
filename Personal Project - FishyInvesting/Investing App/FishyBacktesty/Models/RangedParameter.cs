namespace FishyLibrary.Helpers;

/*
 * Class used to hold variables and functionality. Primary used for backtests.
 * Class so variables could be mutable.
 */
public class RangedParameter
{
    public string Name { get; set; }
    public float StartingValue { get; set; }
    public float EndingValue { get; set; }
    public float StepSize { get; set; }

    public RangedParameter(string name)
    {
        Name = name;
        StartingValue = 0;
        EndingValue = 0;
        StepSize = 1;
    }
    public RangedParameter(string name, float startingValue, float endingValue, float stepSize)
    {
        Name = name;
        StartingValue = startingValue;
        EndingValue = endingValue;
        StepSize = stepSize;
    }
}