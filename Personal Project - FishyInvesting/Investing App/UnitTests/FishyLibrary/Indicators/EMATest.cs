using FishyLibrary.Indicators;

namespace UnitTests;

public class EMATest
{
    [Fact]
    public void oneValue() 
    {
        IIndicator ema = new ExponentialMovingAverage(()=>3);
        double calcVal = ema.GenerateNextValue(2);
        Assert.Equal(1, calcVal);
    }

    [Fact]
    public void twoValue()
    {
        IIndicator ema = new ExponentialMovingAverage(() => 3);
        double calcVal = ema.GenerateNextValue(2);
        Assert.Equal(1, calcVal);
        calcVal = ema.GenerateNextValue(2);
        Assert.Equal(1.5, calcVal);
    }

    [Fact]
    public void threeValue()
    {
        IIndicator ema = new ExponentialMovingAverage(() => 3);
        double calcVal = ema.GenerateNextValue(4);
        Assert.Equal(2, calcVal);
        calcVal = ema.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = ema.GenerateNextValue(6);
        Assert.Equal(4, calcVal);
    }
}
