using Users;
using Innovator.Client.IOM;

public class UserTests(ArasFixture fixture)
{    
    [Fact]
    public void User_can_be_added()
    {
        Innovator.Client.IOM.Innovator inn = fixture.GetAdminInn();
        string loginName = Generators.ShortGUID();
        Console.WriteLine($"Creating user with loginName: {loginName}");
        Item user = User.CreateUser(inn, loginName, "test", loginName, "Doe");
        Assert.False(user.isError());
    }
}