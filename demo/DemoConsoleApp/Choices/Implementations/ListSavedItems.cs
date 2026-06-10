
using Extensions;
using Workflows;

namespace DemoConsoleApp.Choices;

public class ListSavedItems : ChoiceBase
{
    
    public override string Name => "List Saved Items";
    
    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var savedItems = AppSettings.GetSectionSettingAsDictWithTimestamp("SavedItems");
        if (savedItems.Count == 0)
        {
            LogLine("[yellow]No saved items found.[/]");
            return;
        }

        LogLine("[blue]Saved Items:[/]");
        foreach (var kvp in savedItems)
        {
            // LogLine($"[green]{kvp.Key}[/]: {kvp.Value.Value} (Saved: {kvp.Value.SavedDateTime:yyyy-MM-dd HH:mm:ss})");
            var entries = AppSettings.GetSectionSettingAsDictWithTimestamp("SavedItems", kvp.Key);
            LogLine($"[yellow]{kvp.Key}[/]:",includeTimestamp: false);
            foreach (var entry in entries)
            {
                LogLine($"[green]{entry.Key}[/]: {entry.Value.Value} (Saved: {entry.Value.SavedDateTime:yyyy-MM-dd HH:mm:ss})", includeTimestamp: false);
            }
            
        }
        
    }

    
}