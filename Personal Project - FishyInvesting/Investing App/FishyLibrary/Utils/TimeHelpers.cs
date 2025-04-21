namespace FishyLibrary.Utils;

public class TimeHelpers
{
    public static DateTime RoundUp(DateTime dt, TimeSpan d)
    {
        return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
    }

    public static TimeSpan GetTimeToInterval(DateTime timeIn, int interval)
    {
        DateTime time = RoundUp(timeIn, TimeSpan.FromMinutes(interval));
        if (time > timeIn)
        {
            return time - timeIn;
        }

        return timeIn - time;
    }

    public static bool IsMorning(DateTime time)
    {
        return time.TimeOfDay.TotalHours < 9;
    }

    public static bool IsTradingHours(DateTime time)
    {
        return time.TimeOfDay.TotalHours is >= 9 and <= 16;
    }
    
    public static TimeSpan RoundToNearestInterval(TimeSpan timeSpan, int intervalMinutes)
    {
        if (intervalMinutes <= 0)
            throw new ArgumentException("Interval must be greater than zero.", nameof(intervalMinutes));
    
        int totalMinutes = (int)timeSpan.TotalMinutes;

        if (timeSpan.Seconds >= 30)
        {
            totalMinutes += 1;
        }

        int roundedMinutes = (int)(Math.Round(totalMinutes / (double)intervalMinutes) * intervalMinutes);
    
        return TimeSpan.FromMinutes(roundedMinutes);
    }
}