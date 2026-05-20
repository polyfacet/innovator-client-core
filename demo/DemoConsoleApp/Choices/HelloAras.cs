


namespace DemoConsoleApp.Choices;

public class HelloAras : IChoice
{
    public string Name => "Hello Aras";

    public void Execute(Innovator.Client.IOM.Innovator inn)
    {
        AnsiConsole.MarkupLine($"[green]Hello, {inn.GetUser().getProperty("login_name")}![/]");
    }
}