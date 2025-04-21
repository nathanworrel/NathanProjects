using FishyLibrary.Indicators;

namespace UnitTests;

public class MACDIndicatorTest
{
    [Fact]
    public void valueTest()
    {
        IIndicator macd = new MACD(() => 2, () => 4, () => 3);
        double calcVal = macd.GenerateNextValue(2);
        Assert.Equal(0, calcVal);
        calcVal = macd.GenerateNextValue(4);
        Assert.Equal(0, calcVal);
        calcVal = macd.GenerateNextValue(6);
        Assert.Equal(-0.5, calcVal);
        calcVal = macd.GenerateNextValue(8);
        Assert.Equal(-0.75, calcVal);
        calcVal = macd.GenerateNextValue(1);
        Assert.Equal(0.75, calcVal);
    }
}
