


namespace DemoConsoleApp.Choices;

public class Exit : ChoiceBase
{
    public override string Name => "Exit";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        LogLine("[yellow]Exiting application...[/]");
        Environment.Exit(0);
    }
}