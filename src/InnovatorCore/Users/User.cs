using Innovator.Client.IOM;

namespace Users;

public class User
{
    
    public static Item CreateUser(
        Innovator.Client.IOM.Innovator inn,
        string loginName, 
        string password,
         string firstName,
        string lastName)
    {
        var user = inn.newItem("User", "add");
        user.setProperty("login_name", loginName);
        user.setProperty("first_name", firstName);
        user.setProperty("last_name", lastName);
        user.setProperty("password", password);
        var result = user.apply();
        if (result.isError())
        {
            throw new Exception($"Failed to create user: {result.getErrorString()}");
        }
        return result;
    }
}