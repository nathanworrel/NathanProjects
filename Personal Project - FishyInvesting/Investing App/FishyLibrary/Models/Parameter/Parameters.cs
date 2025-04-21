using Newtonsoft.Json;

namespace FishyLibrary.Models.Parameters;

/*
 * Class used to hold all variables and functionality for service paramters
 */
public class Parameters : ParametersBase
{
    public int StrategyTypeId { get; set; }
    
    public StrategyType.StrategyType StrategyType { get; set; }
    
    public List<Strategy.Strategy> Strategies { get; set; }
    
    public Parameters()
    {
        Params = new Dictionary<string, double?>();
    }

    public Parameters(Parameters other)
    {
        Params = new Dictionary<string, double?>(other.Params);
        Id = other.Id;
        StrategyTypeId = other.StrategyTypeId;
    }

    public Parameters(int id, string parameters)
    {
        Id = id;
        try
        {
            Params = JsonConvert.DeserializeObject<Dictionary<string, double?>>(parameters);
        }
        catch (Exception e)
        {
            Params = new Dictionary<string, double?>();
        }
    }

    public Parameters(Dictionary<string, double?> @params)
    {
        Params = @params;
    }

    /*
     * Adds a parameter with the given name and value.
     *
     * Returns nothing
     */
    public void AddParameter(string name, double? value)
    {
        Params.Add(name, value);
    }

    /*
     * Resets all the parameters to the new ones. All parameters in the original parameter set
     * must be present in the new parameters.
     *
     * Returns nothing.
     */
    public void ResetParameters(Parameters newParameters)
    {
        foreach (var param in Params)
        {
            double value = newParameters.GetParameter(param.Key);
            Params[param.Key] = value;
        }
    }

    /*
     * Returns the parameter value with the given name.
     * Throws error if parameter not found or parameter not initialized.
     */
    public double GetParameter(string name)
    {
        if (Params.TryGetValue(name, out double? value))
        {
            return (value ?? throw new Exception("Parameter " + name + " not initalized."));
        }

        throw new Exception("Parameter " + name + " not a valid parameter. ");
    }

    /*
     * Returns all parameters as key value pairs.
     */
    public List<KeyValuePair<string, double?>> GetParameters()
    {
        return Params.ToList();
    }

    public string JsonSerialize()
    {
        return JsonConvert.SerializeObject(Params);
    }
}