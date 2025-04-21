using FishyLibrary.Models.Parameters;
using FishyLibrary.Strategies;
using Newtonsoft.Json;

namespace UnitTests;

public class MACDStrategyTesting
{
    [Fact]
    public void MacDStrategy()
    {
        MacdStrategy MacD = new MacdStrategy();
        Parameters parameters = new Parameters();
        parameters.AddParameter(nameof(MacdStrategy.PeriodShort), 1);
        parameters.AddParameter(nameof(MacdStrategy.PeriodLong), 3);
        parameters.AddParameter(nameof(MacdStrategy.PeriodExponential), 3);
        MacD.ResetStrategy(parameters);

        double calculatedValue = MacD.GenerateSignal([1]);
        Assert.Equal(0, calculatedValue);
        calculatedValue = MacD.GenerateSignal([2]);
        Assert.Equal(0, calculatedValue);
        calculatedValue = MacD.GenerateSignal([3]);
        Assert.Equal(0, calculatedValue);
        calculatedValue = MacD.GenerateSignal([2]);
        Assert.Equal(1, calculatedValue);
        calculatedValue = MacD.GenerateSignal([4]);
        Assert.Equal(0, calculatedValue);

        var intermediateValues = MacD.IntermediateOutputFile();
        Assert.Equal(intermediateValues, MacD.IntermediateOutputFile());
        MacdStrategy newMacdStrategy = new MacdStrategy();
        newMacdStrategy.ResetStrategy(parameters);
        string serializeObject = JsonConvert.SerializeObject(intermediateValues);
        newMacdStrategy.SetIntermediateValues(JsonConvert.DeserializeObject<Dictionary<string,Object>>(serializeObject)!);

        var val1 = MacD.GenerateSignal([4]);
        var val2 = newMacdStrategy.GenerateSignal([4]);
        Assert.Equal(val1, val2);
        Assert.Equal(MacD.IntermediateOutputFile(), newMacdStrategy.IntermediateOutputFile());
    }
}