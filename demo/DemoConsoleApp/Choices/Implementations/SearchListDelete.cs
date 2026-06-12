
using Extensions;
using Workflows;

namespace DemoConsoleApp.Choices;

public class SearchListDelete : ChoiceBase
{
    public override string Name => "Items list Add/List/Delete";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var action = new[] { "Add", "List", "Delete", "Back" };
        var selectedAction = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\n Select action:")
            .AddChoices(action));
        
        if (selectedAction == "Back") return;
        if (selectedAction == "Add") new Search().Execute(inn);
        if (selectedAction == "Delete") new DeleteSavedItem().Execute(inn);
        new ListSavedItems().Execute(inn);
        Execute(inn);

    }   
}
