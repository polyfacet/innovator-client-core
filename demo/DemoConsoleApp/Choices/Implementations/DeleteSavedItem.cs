using Extensions;
using Workflows;

namespace DemoConsoleApp.Choices;

public class DeleteSavedItem : ChoiceBase
{
    public override string Name => "Delete Saved Item";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var sections = AppSettings.GetSectionSettingAsDict("SavedItems");
        if (sections.Count == 0)
        {
            LogLine("[yellow]No saved items to delete.[/]");
            return;
        }

        var sectionPrompt = new SelectionPrompt<string>()
            .Title("\n Select section to delete from:")
            .AddChoices(sections.Keys);
        sectionPrompt.AddChoice("Cancel");

        string sectionSelection = AnsiConsole.Prompt(sectionPrompt);
        if (sectionSelection == "Cancel") return;

        var entries = AppSettings.GetSectionSettingAsDict("SavedItems", sectionSelection);
        if (entries.Count == 0)
        {
            LogLine("[yellow]No leaf entries found under selected section.[/]");
            return;
        }

        var entryPrompt = new MultiSelectionPrompt<string>()
            .Title($"\n Select one or more entries to delete from '{sectionSelection}':")
            .NotRequired()
            .AddChoices(entries.Keys);

        var selectedKeys = AnsiConsole.Prompt(entryPrompt);

        if (selectedKeys == null || selectedKeys.Count == 0)
        {
            LogLine("[yellow]No entries selected. Nothing deleted.[/]");
            return;
        }

        foreach (var key in selectedKeys)
        {
            AppSettings.DeleteSectionSetting("SavedItems", key, sectionSelection);
            LogLine($"[green]Deleted saved item: {sectionSelection}/{key}[/]", includeTimestamp: false);
        }

        AppSettings.SaveChanges();
    }
}