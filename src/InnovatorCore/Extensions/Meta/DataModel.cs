
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
    
    public Packages Packages()
    {
        return new Packages(Inn);
    }
    
}