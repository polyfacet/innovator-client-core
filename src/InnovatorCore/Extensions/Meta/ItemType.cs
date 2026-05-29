
using Extensions;

public class ItemType
{
    private Innovator.Client.IOM.Innovator Inn;

    public Item Item { get; private set; }
    public string Name {get; private set;}

    public ItemType(Innovator.Client.IOM.Innovator inn, string itemTypeName)
    {
        Inn = inn;
        Name = itemTypeName;
        Item = Inn.GetItemByName("ItemType", Name);
    }

    public ArasProperty Property(string propertyName)
    {
        return new ArasProperty(Inn, Name, propertyName);
    }

    public List<ArasProperty> Properties()
    {
        Item propertyRels = Item.GetRelations("Property");
        return propertyRels.ToList().Select(
            p => new ArasProperty(Inn, Name, p.getProperty("name"))
        ).ToList();
    }

    public List<ServerEvent> ServerEvents()
    {
        List<ServerEvent> serverEvents = new();
        Item serverEventItems = Inn.newItem("Server Event", "get");
        serverEventItems.setProperty("source_id", Item.getID());
        serverEventItems = serverEventItems.apply();
        foreach (var serverEventItem in serverEventItems)
        {
            string trigger = serverEventItem.getProperty("server_event");
            DateTime createdOn = DateTime.Parse(serverEventItem.getProperty("created_on"));
            serverEvents.Add(new ServerEvent(Name, serverEventItem.getPropertyAttribute("related_id", "keyed_name") ,trigger, createdOn));
        }
        return serverEvents;
    }

    public bool IsWorkflowEnabled()
    {
        Item rels = Inn.newItem("Allowed Workflow", "get");
        rels.setProperty("source_id", Item.getID());
        rels = rels.apply();
        if (rels.isError()) return false;
        return rels.getItemCount() > 0;
    }

    public List<Workflows.WorkflowMap> WorkflowMaps() 
    {
        Item rels = Inn.newItem("Allowed Workflow", "get");
        rels.setProperty("source_id", Item.getID());
        rels = rels.apply();
        if (rels.isError()) return new List<Workflows.WorkflowMap>();
        List<Workflows.WorkflowMap> workflowMaps = new();
        for(int i = 0; i<rels.getItemCount(); i++) {
            string workflowMapId = rels.getItemByIndex(i).getProperty("related_id");
            workflowMaps.Add(new Workflows.WorkflowMap(Inn, workflowMapId));
        }
        return workflowMaps;
    }
}