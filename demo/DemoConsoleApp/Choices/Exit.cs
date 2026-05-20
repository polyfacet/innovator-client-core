


namespace DemoConsoleApp.Choices;

public class Exit : IChoice
{
    public string Name => "Exit";

    public void Execute(Innovator.Client.IOM.Innovator inn)
    {
        ConsoleLog.Line("[yellow]Exiting application...[/]");
        Environment.Exit(0);
    }
}