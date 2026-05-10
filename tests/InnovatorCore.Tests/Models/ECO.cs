using Extensions;
using Innovator.Client.IOM;
using Workflows;

public class ECO : WorkflowControlledItem
{
    private const string ECO_AFFECTED_ITEM = "Express ECO Affected Item";
    private const string  AFFECTED_ITEM = "Affected Item";

    public ECO(Item item) : base(item) {}

    internal Item AddAffectedItem(Item item, string itemAction)
    {       
        string action = $"<affected_id>{item.getID()}</affected_id>";
        if (itemAction == "Release") action = $"<new_item_id>{item.getID()}</new_item_id>";

        string aml = $@"<AML>
            <Item type='{ECO_AFFECTED_ITEM}' action='add'>
            <related_id>
                <Item type='{AFFECTED_ITEM}'  action='add' >
                <item_action>{itemAction}</item_action>
                {action}
                </Item>
            </related_id>
            <source_id>{SourceItem.getID()}</source_id>
            </Item></AML>";
        Console.WriteLine(aml);
        Item res = Inn.applyAML(aml);
        return res;
    }
}