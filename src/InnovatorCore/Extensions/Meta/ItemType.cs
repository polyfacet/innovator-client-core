
using Extensions;

public class ItemType
{
    private Innovator.Client.IOM.Innovator Inn;
    private string _itemTypeName;

    public Item Item { get; private set; }

    public ItemType(Innovator.Client.IOM.Innovator inn, string itemTypeName)
    {
        Inn = inn;
        _itemTypeName = itemTypeName;
        Item = Inn.GetItemByName("ItemType", _itemTypeName);
    }

    public ArasProperty Property(string propertyName)
    {
        return new ArasProperty(Inn, _itemTypeName, propertyName);
    }

    public List<ArasProperty> Properties()
    {
        Item propertyRels = Item.GetRelations("Property");
        return propertyRels.ToList().Select(
            p => new ArasProperty(Inn, _itemTypeName, p.getProperty("name"))
        ).ToList();
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