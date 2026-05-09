public class InnovatorExtensionTests(ArasFixture fixture)
{
    [Fact]
    public void GetUser_returns_current_User()
    {
        // Arrange
        var inn = fixture.GetAdminInn();
        // Act
        var user = inn.GetUser();
        // Assert
        Assert.NotNull(user);
        Assert.Equal(inn.getUserID(), user.getID());
    }

    [Fact]
    public void GetItem_returns_correct_Item()
    {
        // Arrange
        var inn = fixture.GetAdminInn();
        var itemType = "Variable";

        var createdItem = inn.newItem(itemType, "add");
        string itemName = "Test Variable " + Generators.ShortGUID();
        createdItem.setProperty("name", itemName);
        createdItem = createdItem.apply();
        Assert.False(createdItem.isError(), createdItem.getErrorString());

        // Act
        var retrievedItem = inn.GetItem(itemType, createdItem.getID());

        // Assert
        Assert.NotNull(retrievedItem);
        Assert.Equal(createdItem.getID(), retrievedItem.getID());
        Assert.Equal(itemType, retrievedItem.getType());

        createdItem.apply("delete");
    }

    [Fact]
    public void GetItemByConfigId_returns_correct_Item()
    {
        // Arrange
        var inn = fixture.GetAdminInn();
        var itemType = "Variable";

        var createdItem = inn.newItem(itemType, "add");
        string itemName = "Test Variable " + Generators.ShortGUID();
        createdItem.setProperty("name", itemName);
        createdItem = createdItem.apply();
        Assert.False(createdItem.isError(), createdItem.getErrorString());
        string configId = createdItem.getProperty("config_id");

        // Act
        var retrievedItem = inn.GetItemByConfigId(itemType, configId);

        // Assert
        Assert.NotNull(retrievedItem);
        Assert.Equal(createdItem.getID(), retrievedItem.getID());
        Assert.Equal(itemType, retrievedItem.getType());

        createdItem.apply("delete");
    }

    [Fact]
    public void GetIdentity_returns_Identity_Item_of_the_User()
    {
        // Arrange
        var inn = fixture.GetAdminInn();

        // Act
        var identity = inn.GetIdentity();

        // Assert
        Assert.NotNull(identity);
        Assert.Equal("Identity", identity.getType());
        Assert.Equal(inn.GetUser().getProperty("owned_by_id", "N/A"), identity.getID());
    }
}