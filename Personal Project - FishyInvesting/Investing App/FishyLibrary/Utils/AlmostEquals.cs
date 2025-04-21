using System.Numerics;

namespace FishyLibrary.Utils;

public class AlmostEquals
{
    public static bool Equals<T>(T x, T y, double epsilon = 0.00001) where T : INumber<T>
    {
        try
        {
            Double.TryParse(x.ToString(), out var c);
            Double.TryParse(y.ToString(), out var d);
            if (Math.Abs((c - d)) < epsilon)
            {
                return true;
            }
        
            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}