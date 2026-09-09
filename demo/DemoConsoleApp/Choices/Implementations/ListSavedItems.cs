
using Extensions;
using Workflows;
using Storage.SQLite;

namespace DemoConsoleApp.Choices;

public class ListSavedItems : ChoiceBase
{
    
    public override string Name => "List Saved Items";
    
    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var savedItems = new SavedItems(InitSqLite.GetConnection()).GetItems();
        if (savedItems.Count == 0)
        {
            LogLine("[yellow]No saved items found.[/]");
            return;
        }

        LogLine("[blue]Saved Items:[/]");
        foreach (var itemTypeGroup in savedItems.GroupBy(item => item.ItemType))
        {
            LogLine($"[yellow]{itemTypeGroup.Key}[/]:", includeTimestamp: false);
            foreach (var item in itemTypeGroup)
            {
                LogLine($"[green]{item.Name}[/]: {item.ConfigId} (Saved: {item.SavedDateTime:yyyy-MM-dd HH:mm:ss})", includeTimestamp: false);
            }
        }
    }
}