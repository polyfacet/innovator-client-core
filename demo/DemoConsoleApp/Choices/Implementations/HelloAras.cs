


namespace DemoConsoleApp.Choices;

public class HelloAras : ChoiceBase
{
    public override string Name => "Hello Aras";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        AnsiConsole.MarkupLine($"[green]Hello, {inn.GetUser().getProperty("login_name")}![/]");
    }
}