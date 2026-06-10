
using Extensions;
using Workflows;

namespace DemoConsoleApp.Choices;

public class Search : ChoiceBase
{
    public override string Name => "Search Items";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        string itemTypeName = AnsiConsole.Ask<string>("Enter Item Type:");
        string searchTerm = AnsiConsole.Ask<string>("Enter search term for item name:");
        var items = inn.ApplyAML($@"<AML><Item type='{itemTypeName}' action='get'><name condition='like'>{searchTerm}</name></Item></AML>");
        if (items.isError())
        {
            LogLine($"[red]Error executing search: {items.getErrorString()}[/]");
            return;
        }
        List<Item> itemsList = items.ToList();
        if (itemsList.Count == 0)
        {
            LogLine($"[yellow]No items found matching criteria.[/]");
            return;
        }

        Dictionary<string, Item> itemDict = itemsList.ToDictionary(i => i.getProperty("name"), i => i);

        var mSearchResults = new MultiSelectionPrompt<string>()
            .PageSize(10)
            .Title("Select items to view details:")
            .InstructionsText("[grey](Press [blue]<space>[/] to toggle a selection, [green]<enter>[/] to accept)[/]")
            .AddChoices(itemsList.Select(i => i.getProperty("name")).ToArray());
        var favorites = AnsiConsole.Prompt(mSearchResults);
        foreach (var fav in favorites)
        {
            var item = itemDict[fav];
            AppSettings.AddOrUpdateSectionSetting("SavedItems", item.getProperty("name"), item.getProperty("config_id"), itemTypeName);
            LogLine($"[blue]{item.getProperty("name")}[/], Modified On: [blue]{item.getProperty("modified_on")}[/], Config ID: [blue]{item.getProperty("config_id")}[/]", includeTimestamp: false);
        }
        AppSettings.SaveChanges();
        LogLine("[green]Selected items saved to settings![/]");

        
    }

    
}