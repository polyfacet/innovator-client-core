
public class ItemBase : InnovatorBase
{
    protected readonly Item Item;
    public ItemBase(Item item) : base(item.getInnovator())
    {
        Item = item;
    }
}


