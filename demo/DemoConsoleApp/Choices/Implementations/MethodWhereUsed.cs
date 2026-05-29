


using Innovator.Client.QueryModel.Functions;

namespace DemoConsoleApp.Choices;

public class MethodWhereUsed : ChoiceBase
{
    public override string Name => "Method Where Used";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        Item methodItem = AskForMethodItem(inn);
        if (methodItem.isError()) return;
        Method method = new Method(methodItem);
        List<ServerEvent> serverEvents = method.UsedIn().ServerEvents();
        if (serverEvents.Count > 0) PrintTable(serverEvents);
        List<Action> actions = method.UsedIn().Actions();
        if (actions.Count >0) PrintActions(actions);
        List<string> genericUsedIn = method.UsedIn().GenericUsedIn();
        if (genericUsedIn.Count > 0)        {
            LogLine("Used generically in:");
            genericUsedIn.ForEach(s => LogLine($"[blue]{s}[/]", includeTimestamp: false));
        }
    }

    private Item AskForMethodItem(Innovator.Client.IOM.Innovator inn)
    {
        string methodName = AnsiConsole.Ask<string>("Enter Method Name:");
        Item methodItem = inn.GetItemByName("Method", methodName);
        if (!methodItem.isError()) return methodItem;
        
        LogLine($"[yellow]Method {methodName} not found.[/]");
        if (AnsiConsole.Confirm("Try new method name?")) return AskForMethodItem(inn);
        return inn.newError("Aborted");
        
    }

    private void PrintTable(List<ServerEvent> serverEvents)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Green)
            .AddColumn(new TableColumn("[u]Item Type[/]"))
            .AddColumn(new TableColumn("[u]Event[/]"))
            .AddColumn(new TableColumn("[u]Created On[/]"));
        foreach (var serverEvent in serverEvents)
        {
            table.AddRow(serverEvent.ItemType, serverEvent.ServerEventTrigger, serverEvent.CreatedOn.ToString("s"));
        }

        AnsiConsole.Write(
            Align.Left(table));     
    }

    private void PrintActions(List<Action> actions)
    {
        foreach(var action in actions)
        {
            LogLine($"Used in Action [blue]{action.Name} [/]", false);
            foreach (var itemType in action.ItemTypes)
            {
                LogLine($"\t In item type [blue]{itemType.Name}[/]", false);
            }
        }
    }
}