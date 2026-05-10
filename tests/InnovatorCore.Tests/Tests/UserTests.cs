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
        Item user = new User(inn).CreateUser(loginName, "test", loginName, "Doe");
        Assert.False(user.isError());
    }

    [Fact]
    public void Test_UserExists_returns_true_for_existing_user() {
        Innovator.Client.IOM.Innovator inn = fixture.GetAdminInn();
        string loginName = inn.GetUser().getProperty("login_name", "N/A");
        bool exists = new User(inn).UserExists(loginName);
        Assert.True(exists);
    }

    [Fact]
    public void Test_IsMemberOf() {
        Innovator.Client.IOM.Innovator inn = fixture.GetAdminInn();
        User user = new User(inn);
        Item groupIdentity = inn.GetItemByName("Identity", "Administrators");
        Assert.True(user.IsMemberOf(groupIdentity));
    }

    [Fact]
    public void Test_UserIsEnabled() {
        Innovator.Client.IOM.Innovator inn = fixture.GetAdminInn();
        User user = new User(inn);
        Assert.True(user.UserIsEnabled(inn.GetUser().getProperty("login_name", "N/A")));        
    }

    [Fact]
    public void Test_Add_Remove_MemberOf() {
        Innovator.Client.IOM.Innovator inn = fixture.GetAdminInn();

        // Create a new identity to use as a group
        string identityName = "TestIdentity_" + Generators.ShortGUID();
        Item identity = inn.newItem("Identity", "add");
        identity.setProperty("name", identityName);
        identity = identity.apply();

        User user = new User(inn);
        // Add user as member of the identity created above, then verify
        user.AddUserAsMember(inn.GetUser(), identityName);
        Assert.True(user.IsDirectMemberOf(identity));

        // Remove user as member of the identity, then verify
        user.RemoveUserAsMember(inn.GetUser(), identityName);
        Assert.False(user.IsDirectMemberOf(identity));

        // Clean up identity created for test
        identity.setAction("delete");
        identity.apply();
    }
        
}