namespace Sandbox;
class Sandbox
{
    static void Main()
    {
        // Call the main function
        DateTime temp3 = new DateTime(2020, 1, 1, 8, 30, 0);
        var temp4 = temp3.ToUniversalTime();
        Console.WriteLine(temp4);
    }
}
