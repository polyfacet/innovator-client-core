
public class DataModel
{
    private Innovator.Client.IOM.Innovator Inn;
    
    public DataModel(Innovator.Client.IOM.Innovator inn)
    {
        Inn = inn;
    }

    public ItemType ItemType(string itemTypeName)
    {
        return new ItemType(Inn, itemTypeName);
    }

    public Method Method(Item methodItem)
    {
        return new Method(methodItem);
    }

    public Packages Packages()
    {
        return new Packages(Inn);
    }
    
}

