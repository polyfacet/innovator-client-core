public class Generators
{

    public static string ShortGUID()
    {
        return Guid.NewGuid().ToString().Substring(0, 8);
    }

    public static string RandomEmail()
    {
        return $"{RandomString(8)}@example.com";
    }
    
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    } 
    
}