using Users;
using Innovator.Client.IOM;
using System.Security.Cryptography;
public class UserTests
{
    [Fact]
    public void User_can_be_added()
    {
        Innovator.Client.IOM.Innovator inn = DefaultEnvConfig.GetAdminSession();
        string loginName = Generators.ShortGUID();
        Item user = User.CreateUser(inn, loginName, "test", loginName, "Doe");
        Assert.False(user.isError());
    }
}