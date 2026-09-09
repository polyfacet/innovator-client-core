using Spectre.Console;
using Storage.SQLite;

namespace DemoConsoleApp.Choices;

public class SwitchArasConnection : ChoiceBase
{
    public override string Name => "Switch Aras Connection";

    public Innovator.Client.IOM.Innovator? ConnectedInnovator { get; private set; }

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        ConnectedInnovator = null;

        var connections = new ArasConnections(InitSqLite.GetConnection()).GetConnections();
        if (connections.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No saved connections found.[/]");
            return;
        }

        var selectedName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n Select a connection:")
                .EnableSearch()
                .AddChoices(connections.OrderBy(connection => connection.SortOrder).Select(connection => connection.Name)));

        var selectedConnection = connections.First(connection => connection.Name == selectedName);
        var connectedInnovator = InnovatorSession.CreateInnovatorSession(selectedConnection);
        if (connectedInnovator == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to switch connection. The current connection is unchanged.[/]");
            return;
        }

        ConnectedInnovator = connectedInnovator;
        AnsiConsole.MarkupLine($"[green]Switched to connection: {Markup.Escape(selectedConnection.Name)}[/]");
    }
}