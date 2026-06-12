using Innovator.Client.QueryModel.Functions;
using Storage.SQLite;

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
        if (AnsiConsole.Confirm("Try another method?")) Execute(inn);
    }

    private Item AskForMethodItem(Innovator.Client.IOM.Innovator inn)
    {
        var savedMethods = new SavedItems(InitSqLite.GetConnection()).GetItems("Method");
        foreach (var savedMethod in savedMethods)
        {
            LogLine($"Saved Method: [blue]{savedMethod.Name}[/], Config ID: [blue]{savedMethod.ConfigId}[/]", includeTimestamp: false);
        }
        if (savedMethods.Count > 0)
        {
            var methodNames = savedMethods.Select(method => method.Name).ToList();
            methodNames.Add("Enter new method name");
            var methodPrompt = new SelectionPrompt<string>()
                .Title("Select a saved method or enter a new one:")
                .AddChoices(methodNames);
            string selectedMethod = AnsiConsole.Prompt(methodPrompt);
            if (selectedMethod != "Enter new method name")
            {
                string configId = savedMethods.Single(method => method.Name == selectedMethod).ConfigId;
                Console.WriteLine($"Retrieving method {selectedMethod} with config ID {configId}...");
                Item methodItem1 = inn.GetItemByConfigId("Method", configId);
                if (!methodItem1.isError()) return methodItem1;
                LogLine($"[yellow]Failed to retrieve method {selectedMethod} by config ID. It may have been deleted or the config ID is invalid.[/]");
            }
        }

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