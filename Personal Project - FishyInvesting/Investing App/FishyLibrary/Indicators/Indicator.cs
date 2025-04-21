
namespace FishyLibrary.Indicators;

public interface IIndicator
{
    /*
     * Updates strategy with new value
     */
     void UpdateIndicator(double newPrice);

    /*
     * Gets new indicator value
     */
     double GetValue();

    /*
     *  Generates an indicator value
     */
     double GenerateNextValue(double newPrice)
     {
        UpdateIndicator(newPrice);
        return GetValue();
     }

    /*
     *  Resets the indicator for future use
     */
    void ResetIndicator();
    
    /*
     *  Gets the intermediary data
     */
    public Dictionary<string, object> IntermediateOutputFile();
    
    /*
     * Sets the intermediary values
     */
    public void SetIntermediateValues(Dictionary<string, object> intermediateData);
}
