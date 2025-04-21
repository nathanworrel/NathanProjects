using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Parameters;

namespace FishyLibrary.Strategies;

public interface IStrategy
{
    /*
     * The name of the strategy. Used to track what strategy
     * authorizes each trade.
     */
    
    /*
     * Holds all the parameters for a strategy and their values
     */
    public Parameters Parameters { get; }
    
    /*
     * Gets the number of Products needed for a strategy
     */
    public int NumProducts { get; }
    
    /*
     * Gets the MaxPeriod of data
     */
    public double MaxPeriod { get; }
    
    /*
     * Generates a signal
     *
     * Returns percentage of money that should be in the market. 1 if 100% in the market, returns 0 if not.
     */
    public abstract double GenerateSignal(List<double> price);

    /*
     * Resets the strategy and updates the parameters
     *
     * Returns Nothing
     */
    public abstract void ResetStrategy(Parameters parameters);
    
    /*
     *  Gets the intermediary data
     */
    Dictionary<string, object> IntermediateOutputFile();
    
    /*
     *  Gets the intermediary data
     */
    public void SetIntermediateValues(Dictionary<string, object> intermediateData);
}