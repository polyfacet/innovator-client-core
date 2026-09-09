using Spectre.Console;

namespace DemoConsoleApp.Choices;

public class DemosChoice : ChoiceBase
{
    public override string Name => "Demos";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var choices = new IChoice[]
        {
            new ItemTypeInfo(),
            new PackageManagementDemo(),
            new CreateDeleteUser(),
            new ReleasePartViaECO(),
            new BackChoice()
        };

        var selectedChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n Select a demo:")
                .AddChoices(choices.Select(choice => choice.Name)));

        if (selectedChoice == "Back")
        {
            return;
        }

        choices.First(choice => choice.Name == selectedChoice).Execute(inn);
        Execute(inn);
    }

    private sealed class BackChoice : ChoiceBase
    {
        public override string Name => "Back";

        public override void Execute(Innovator.Client.IOM.Innovator inn)
        {
        }
    }
}