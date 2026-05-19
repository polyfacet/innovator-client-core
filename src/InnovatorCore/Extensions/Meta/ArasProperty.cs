using Extensions;

public class ArasProperty
{
    private Innovator.Client.IOM.Innovator Inn;
    private string _itemTypeName;
    private string _propertyName;

    private Item? _propertyRelation;
    public Item? PropertyRelation
    { 
        get 
        {
            if (!_dataFetched)
            {
                List<Item> propertyRelations = GetPropertyRelations();
                _propertyRelation = propertyRelations.FirstOrDefault(
                    p => p.getProperty("name") == _propertyName);
            } 
            return _propertyRelation;
        }
    }
    

    private bool _dataFetched = false;

    public ArasProperty(Innovator.Client.IOM.Innovator inn, string itemTypeName, string propertyName)
    {
        Inn = inn;
        _itemTypeName = itemTypeName;
        _propertyName = propertyName;
    }

    public bool Exists()
    {
        if (PropertyRelation == null) return false;
        return true;
    }

    public string DataType()
    {
        if (PropertyRelation == null) return "N/A";
        return PropertyRelation.getProperty("data_type");
    }

    private List<Item> GetPropertyRelations()
    {
        Item itemType = Inn.GetItemByName("ItemType", _itemTypeName);
        Item propertyRels = itemType.GetRelations("Property");
        _dataFetched = true;
        return propertyRels.ToList();
    }

    public bool IsRequired()
    {
        if (PropertyRelation == null) return false;
        return PropertyRelation.getProperty("is_required") == "1";
    }

    public bool HiddenOnMainSearch()
    {
        if (PropertyRelation == null) return true;
        return PropertyRelation.getProperty("is_hidden") == "1";
    }

    public bool HiddenOnRelationships()
    {
        if (PropertyRelation == null) return true;
        return PropertyRelation.getProperty("is_hidden2") == "1";
    }

    public int SortOrder()
    {
        if (PropertyRelation == null) return 0;
        return int.Parse(PropertyRelation.getProperty("sort_order"));
    }

}