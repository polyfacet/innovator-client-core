
using Extensions;
using Innovator.Client.IOM;

public class DataModelTests
{
    private readonly ArasFixture _fixture;

    public DataModelTests(ArasFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void DataModel_Part_Demo_Test()
    {
        Innovator.Client.IOM.Innovator inn = _fixture.GetAdminInn();
        
        ItemType partItemType =  inn.DataModel().ItemType("Part");
        Assert.True(partItemType.Item.LastModified() > DateTime.MinValue);
        Console.WriteLine("Part ItemType last modified: " + partItemType.Item.LastModified());

        bool exists = partItemType.Property("item_number").Exists();
        Assert.True(exists);
        
        ArasProperty prop = partItemType.Property("item_number"); 
        
        string propertyType = prop.DataType();
        Assert.Equal("String", propertyType, ignoreCase: true);
        Assert.True(prop.IsRequired());
        Assert.False(prop.HiddenOnMainSearch());
        Assert.False(prop.HiddenOnRelationships());
        Assert.True(prop.SortOrder() > 0);

        partItemType.Properties().ForEach(p => {
            Console.WriteLine($"Property: {p.PropertyRelation?.getProperty("name")}, type = {p.DataType()}"); //, required = {p.IsRequired()}");
        });

    }
}