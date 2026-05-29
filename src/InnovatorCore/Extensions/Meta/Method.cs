public class Method : ItemBase
{
    public Item MethodItem { get; }

    public Method(Item methodItem) : base(methodItem)
    {
        MethodItem = methodItem;
    }

    public UsedIn UsedIn()
    {
        return new UsedIn(MethodItem);
    } 
}

public class UsedIn : ItemBase
{
    public UsedIn(Item methodItem) : base(methodItem)  {}
    public List<ServerEvent> ServerEvents()
    {
        List<ServerEvent> serverEvents = new();
        Item serverEventItems = Inn.newItem("Server Event", "get");
        serverEventItems.setProperty("related_id", Item.getID());
        serverEventItems = serverEventItems.apply();
        foreach (Item serverEvent in serverEventItems.ToList())
        {
            string itemType = serverEvent.getPropertyAttribute("source_id", "keyed_name");
            string trigger = serverEvent.getProperty("server_event");
            DateTime createdOn = DateTime.Parse(serverEvent.getProperty("created_on"));
            serverEvents.Add(new ServerEvent(itemType, Item.getProperty("name"),trigger, createdOn));
        }
        return serverEvents;
    }

    public List<Action> Actions()
    {
        List<Action> actions = new();
        Item actionItems = Inn.newItem("Action", "get");
        actionItems.setProperty("method", Item.getID());
        actionItems = actionItems.apply();
        foreach (var actionItem in actionItems)
        {
            string actionName = actionItem.getProperty("name");
            Item itemActions = Inn.newItem("Item Action", "get");
            itemActions.setProperty("related_id", actionItem.getID());
            itemActions = itemActions.apply();
            List<ItemType> itemTypes = new();
            foreach (var itemAction in itemActions)
            {
                string itemTypeName = itemAction.getPropertyAttribute("source_id", "keyed_name");
                ItemType itemType = new ItemType(Inn, itemTypeName);
                itemTypes.Add(itemType);
            }
            actions.Add(new Action(actionName, itemTypes));
        }
        return actions;
    }

    public List<string> GenericUsedIn()
    {
        string aml = $@"<AML>
            <Item action='getItemWhereUsed' id='{Item.getID()}' type='Method'></Item>
        </AML>";
        List<string> usedInList = new();
        Item amlResult = Inn.applyAML(aml);
        if (amlResult.isError()) return usedInList;
        Item relatedItems =  amlResult.getItemsByXPath("//Item/relatedItems/Item");
        foreach (var relatedItem in relatedItems.ToList())
        {
            string keyedName = relatedItem.getAttribute("keyed_name");
            string id = relatedItem.getID();
            // string type = relatedItem.getType();
            // usedInList.Add($"{type}: {keyedName} (ID: {id})");
            usedInList.Add($"{keyedName} (ID: {id})");
        }
        return usedInList;
    }
}

public record ServerEvent(
    string ItemType,
    string MethodName,
    string ServerEventTrigger, 
    DateTime CreatedOn
);

public record Action(
    string Name,
    List<ItemType> ItemTypes
);
