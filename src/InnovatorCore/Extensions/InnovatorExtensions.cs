
public static class InnovatorExtensions
{
    public static Item GetUser(this Innovator.Client.IOM.Innovator inn)
    {
        return inn.GetItem("User", inn.getUserID());
    }

    public static Item GetItem(this Innovator.Client.IOM.Innovator inn, string itemType, string itemId)
    {
        Item item = inn.newItem(itemType, "get");
        item.setID(itemId);
        return item.apply();
    }

    public static Item GetItem(this Innovator.Client.IOM.Innovator inn, string itemType, string itemId, string select)
    {
        Item item = inn.newItem(itemType, "get");
        item.setID(itemId);
        item.setProperty("select", select);
        return item.apply();
    }

    public static Item GetItemByConfigId(this Innovator.Client.IOM.Innovator inn,
        string itemType,
        string config_id
    ) {
        Item item = inn.newItem(itemType, "get");
        item.setProperty("config_id", config_id);
        item = item.apply();
        return item;
    }
    public static Item GetIdentity(this Innovator.Client.IOM.Innovator inn) {
        Item user = inn.GetItem("User", inn.getUserID(), "owned_by_id");
        if (user.isError()) return user;
        return inn.GetItem("Identity", user.getProperty("owned_by_id", "N/A"));
    }
}