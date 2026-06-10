using Spectre.Console;

namespace DemoConsoleApp.Choices;

public class SavedSettingsExample : ChoiceBase, ISaveable
{
    private AppSettings? _appSettings;

    public override string Name => "Saved Settings Example";

    public void InitializeSettings(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        if (_appSettings == null)
        {
            LogLine("[red]AppSettings not initialized[/]");
            return;
        }

        LogLine("[blue]=== Saved Settings Example ===[/]");

        // Example 1: Get saved items
        LogLine("[yellow]Retrieving saved methods...[/]");
        var methods = AppSettings.GetSectionSettingAsDictWithTimestamp("SavedItems", "Methods");
        
        foreach (var kvp in methods)
        {
            LogLine($"  {kvp.Key}: {kvp.Value.Value} (Saved: {kvp.Value.SavedDateTime:yyyy-MM-dd HH:mm:ss})");
        }

        // Example 2: Add a new saved item
        LogLine("[yellow]Adding new saved method...[/]");
        AppSettings.AddOrUpdateSectionSetting("SavedItems", "Method4", "PackageManagementDemo", "Methods");
        AppSettings.SaveChanges();
        LogLine("[green]New method saved![/]");

        // Example 3: Verify the save
        var updatedMethods = AppSettings.GetSectionSettingAsDictWithTimestamp("SavedItems", "Methods");
        LogLine("[yellow]Updated methods:[/]");
        foreach (var kvp in updatedMethods)
        {
            LogLine($"  {kvp.Key}: {kvp.Value.Value} (Saved: {kvp.Value.SavedDateTime:yyyy-MM-dd HH:mm:ss})");
        }

        LogLine("[green]Example completed![/]");
    }
}
