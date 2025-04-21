using FishyLibrary.Utils;

namespace UnitTests.Utils;

public class TimeHelpersTest
{
    [Fact]
    public void TimeHelpers_RoundUp_Halfway()
    {
        DateTime received = TimeHelpers.RoundUp(
            new DateTime(2024, 12, 12, 5, 0, 30), 
            new TimeSpan(0, 1, 0));
        DateTime expected = new DateTime(2024, 12, 12, 5, 1, 0);
        Assert.Equal(received, expected);
    }
    
    [Fact]
    public void TimeHelpers_RoundUp_Close()
    {
        DateTime received = TimeHelpers.RoundUp(
            new DateTime(2024, 12, 12, 5, 0, 59), 
            new TimeSpan(0, 1, 0));
        DateTime expected = new DateTime(2024, 12, 12, 5, 1, 0);
        Assert.Equal(received, expected);
    }
    
    [Fact]
    public void TimeHelpers_RoundUp_Far()
    {
        DateTime received = TimeHelpers.RoundUp(
            new DateTime(2024, 12, 12, 5, 0, 01), 
            new TimeSpan(0, 1, 0));
        DateTime expected = new DateTime(2024, 12, 12, 5, 1, 0);
        Assert.Equal(received, expected);
    }
    
    [Fact]
    public void TimeHelpers_RoundUp_Exact()
    {
        DateTime received = TimeHelpers.RoundUp(
            new DateTime(2024, 12, 12, 5, 1, 0), 
            new TimeSpan(0, 1, 0));
        DateTime expected = new DateTime(2024, 12, 12, 5, 1, 0);
        Assert.Equal(received, expected);
    }

    [Fact]
    public void TimeHelpers_GetTimeToInterval_Close()
    {
        TimeSpan expected = new TimeSpan(0, 1, 0);
        TimeSpan received = TimeHelpers.GetTimeToInterval(
            new DateTime(2024, 12, 12, 5, 19, 0), 10);
        Assert.Equal(expected, received);
    }
    
    [Fact]
    public void TimeHelpers_GetTimeToInterval_Far()
    {
        TimeSpan expected = new TimeSpan(0, 8, 45);
        TimeSpan received = TimeHelpers.GetTimeToInterval(
            new DateTime(2024, 12, 12, 5, 11, 15), 10);
        Assert.Equal(expected, received);
    }
    
    [Fact]
    public void TimeHelpers_GetTimeToInterval_Exact()
    {
        TimeSpan expected = new TimeSpan(0, 0, 0);
        TimeSpan received = TimeHelpers.GetTimeToInterval(
            new DateTime(2024, 12, 12, 5, 10, 0), 10);
        Assert.Equal(expected, received);
    }

    [Fact]
    public void TimeHelpers_IsMorning_Close()
    {
        Assert.True(TimeHelpers.IsMorning(new DateTime(2024, 12, 12, 8, 59, 59)));
    }
    
    [Fact]
    public void TimeHelpers_IsMorning_Not()
    {
        Assert.False(TimeHelpers.IsMorning(new DateTime(2024, 12, 12, 14, 59, 59)));
    }
    
    [Fact]
    public void TimeHelpers_IsMorning_Far()
    {
        Assert.True(TimeHelpers.IsMorning(new DateTime(2024, 12, 12, 0, 0, 0)));
    }
    
    [Fact]
    public void TimeHelpers_IsMorning_Exact()
    {
        Assert.False(TimeHelpers.IsMorning(new DateTime(2024, 12, 12, 9, 0, 0)));
    }

    [Fact]
    public void TimeHelpers_IsTradingHours_ExactStart()
    {
        Assert.True(TimeHelpers.IsTradingHours(new DateTime(2024, 12, 12, 9, 0, 0)));
    }

    [Fact]
    public void TimeHelpers_IsTradingHours_ExactEnd()
    {
        Assert.True(TimeHelpers.IsTradingHours(new DateTime(2024, 12, 12, 16, 0, 0)));
    }

    [Fact]
    public void TimeHelpers_IsTradingHours_BeforeTradingHours()
    {
        Assert.False(TimeHelpers.IsTradingHours(new DateTime(2024, 12, 12, 8, 59, 0)));
    }

    [Fact]
    public void TimeHelpers_IsTradingHours_AfterTradingHours()
    {
        Assert.False(TimeHelpers.IsTradingHours(new DateTime(2024, 12, 12, 16, 1, 0)));
    }

    [Fact]
    public void TimeHelpers_IsTradingHours_MidTradingHours()
    {
        Assert.True(TimeHelpers.IsTradingHours(new DateTime(2024, 12, 12, 11, 0, 0)));
    }
    
    [Fact]
    public void TimeHelpers_RoundToNearestInterval_RoundUp()
    {
        var timeSpan = new TimeSpan(10, 7, 0);
        var result = TimeHelpers.RoundToNearestInterval(timeSpan, 10);
        Assert.Equal(new TimeSpan(10, 10, 0), result);
    }

    [Fact]
    public void TimeHelpers_RoundToNearestInterval_RoundDown()
    {
        var timeSpan = new TimeSpan(10, 4, 31);
        var result = TimeHelpers.RoundToNearestInterval(timeSpan, 10);
        Assert.Equal(new TimeSpan(10, 0, 0), result);
    }

    [Fact]
    public void TimeHelpers_RoundToNearestInterval_ExactMultiple()
    {
        var timeSpan = new TimeSpan(10, 30, 0);
        var result = TimeHelpers.RoundToNearestInterval(timeSpan, 10);
        Assert.Equal(new TimeSpan(10, 30, 0), result);
    }

    [Fact]
    public void TimeHelpers_RoundToNearestInterval_OneMinuteInterval()
    {
        var timeSpan = new TimeSpan(10, 5, 31);
        var result = TimeHelpers.RoundToNearestInterval(timeSpan, 1);
        Assert.Equal(new TimeSpan(10, 6, 0), result);
    }

    [Fact]
    public void TimeHelpers_RoundToNearestInterval_ZeroInterval_ThrowsException()
    {
        var timeSpan = new TimeSpan(10, 5, 0);
        Assert.Throws<ArgumentException>(() => TimeHelpers.RoundToNearestInterval(timeSpan, 0));
    }

    [Fact]
    public void TimeHelpers_RoundToNearestInterval_NegativeInterval_ThrowsException()
    {
        var timeSpan = new TimeSpan(10, 5, 0);
        Assert.Throws<ArgumentException>(() => TimeHelpers.RoundToNearestInterval(timeSpan, -5));
    }
}