using FishyLibrary.Utils;

namespace UnitTests.Utils;

public class AlmostEqualTest
{
    [Fact]
    public void AlmostEquals_DoubleEqual()
    {
        Assert.True(AlmostEquals.Equals<double>(1, 1));
        Assert.True(AlmostEquals.Equals<double>(1, 1.000001));
        Assert.True(AlmostEquals.Equals<double>(1, 1.00001, 0.0001));
    }
    
    [Fact]
    public void AlmostEquals_DecimalEqual()
    {
        Assert.True(AlmostEquals.Equals<decimal>(1, 1));
        Assert.True(AlmostEquals.Equals<decimal>(1, 1.000001m));
        Assert.True(AlmostEquals.Equals<decimal>(1, 1.00001m, 0.0001));
    }
    
    [Fact]
    public void AlmostEquals_FloatEqual()
    {
        Assert.True(AlmostEquals.Equals<float>(1, 1));
        Assert.True(AlmostEquals.Equals<float>(1, 1.000001f));
        Assert.True(AlmostEquals.Equals<float>(1, 1.00001f, 0.0001));
    }
    
    [Fact]
    public void AlmostEquals_DoubleNotEqual()
    {
        Assert.False(AlmostEquals.Equals<double>(1, 2));
        Assert.False(AlmostEquals.Equals<double>(1, 1.0009));
        Assert.False(AlmostEquals.Equals<double>(1, 1.009, 0.0001));
    }
    
    [Fact]
    public void AlmostEquals_DecimalNotEqual()
    {
        Assert.False(AlmostEquals.Equals<decimal>(1, 2));
        Assert.False(AlmostEquals.Equals<decimal>(1, 1.0009m));
        Assert.False(AlmostEquals.Equals<decimal>(1, 1.009m, 0.0001));
    }
    
    [Fact]
    public void AlmostEquals_FloatNotEqual()
    {
        Assert.False(AlmostEquals.Equals<float>(1, 2));
        Assert.False(AlmostEquals.Equals<float>(1, 1.0009f));
        Assert.False(AlmostEquals.Equals<float>(1, 1.009f, 0.0001));
    }
}