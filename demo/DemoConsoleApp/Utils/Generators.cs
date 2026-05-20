public class Generators
{

    public static string ShortGUID()
    {
        return Guid.NewGuid().ToString().Substring(0, 8);
    }
}