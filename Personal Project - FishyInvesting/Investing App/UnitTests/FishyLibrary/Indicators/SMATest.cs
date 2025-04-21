using FishyLibrary.Indicators;

namespace UnitTests;

public class SMATest
{
    [Fact]
    public void oneValueTest() 
    {
        IIndicator sma = new SimpleMovingAverage(() => 2 );
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal( 2, calcVal );
    }

    [Fact]
    public void twoValueTest()
    {
        IIndicator sma = new SimpleMovingAverage(() => 2);
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = sma.GenerateNextValue(4);
        Assert.Equal(3, calcVal);
    }

    [Fact]
    public void threeValueTest()
    {
        IIndicator sma = new SimpleMovingAverage(() => 2);
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = sma.GenerateNextValue(4);
        Assert.Equal(3, calcVal);
        calcVal = sma.GenerateNextValue(6);
        Assert.Equal(5, calcVal);
    }

    [Fact]
    public void fourValueTest()
    {
        IIndicator sma = new SimpleMovingAverage(() => 3);
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = sma.GenerateNextValue(4);
        Assert.Equal(3, calcVal);
        calcVal = sma.GenerateNextValue(6);
        Assert.Equal(4, calcVal);
        calcVal = sma.GenerateNextValue(8);
        Assert.Equal(6, calcVal);
    }

    [Fact]
    public void fiveValueTest()
    {
        IIndicator sma = new SimpleMovingAverage(() => 3);
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = sma.GenerateNextValue(4);
        Assert.Equal(3, calcVal);
        calcVal = sma.GenerateNextValue(6);
        Assert.Equal(4, calcVal);
        calcVal = sma.GenerateNextValue(8);
        Assert.Equal(6, calcVal);
        calcVal = sma.GenerateNextValue(1);
        Assert.Equal(5, calcVal);
    }

    [Fact]
    public void resetTest()
    {
        IIndicator sma = new SimpleMovingAverage(() => 2);
        double calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);

        sma.ResetIndicator();
        calcVal = sma.GenerateNextValue(2);
        Assert.Equal(2, calcVal);
        calcVal = sma.GenerateNextValue(4);
        Assert.Equal(3, calcVal);
    }
}
