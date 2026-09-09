using Storage.SQLite;

namespace DemoConsoleApp.Choices;

public class DeleteSavedItem : ChoiceBase
{
    public override string Name => "Delete Saved Item";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var savedItems = new SavedItems(InitSqLite.GetConnection());
        var sections = savedItems.GetItemTypes();
        if (sections.Count == 0)
        {
            LogLine("[yellow]No saved items to delete.[/]");
            return;
        }

        var sectionPrompt = new SelectionPrompt<string>()
            .Title("\n Select section to delete from:")
            .EnableSearch()
            .AddChoices(sections);
        sectionPrompt.AddChoice("Cancel");

        string sectionSelection = AnsiConsole.Prompt(sectionPrompt);
        if (sectionSelection == "Cancel") return;

        var entries = savedItems.GetItems(sectionSelection);
        if (entries.Count == 0)
        {
            LogLine("[yellow]No leaf entries found under selected section.[/]");
            return;
        }

        var entryPrompt = new MultiSelectionPrompt<string>()
            .Title($"\n Select one or more entries to delete from '{sectionSelection}':")
            .NotRequired()
            .AddChoices(entries.Select(entry => entry.Name));

        var selectedKeys = AnsiConsole.Prompt(entryPrompt);

        if (selectedKeys == null || selectedKeys.Count == 0)
        {
            LogLine("[yellow]No entries selected. Nothing deleted.[/]");
            return;
        }

        foreach (var key in selectedKeys)
        {
            savedItems.Delete(sectionSelection, key);
            LogLine($"[green]Deleted saved item: {sectionSelection}/{key}[/]", includeTimestamp: false);
        }
    }
}