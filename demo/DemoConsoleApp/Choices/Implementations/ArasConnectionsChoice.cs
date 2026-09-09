using Spectre.Console;
using Storage.SQLite;

namespace DemoConsoleApp.Choices;

public class ArasConnectionsChoice : ChoiceBase
{
    public override string Name => "Aras Connections";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        var actions = new[] { "List", "Add", "Delete", "Back" };
        var selectedAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n Select connection action:")
                .AddChoices(actions));

        switch (selectedAction)
        {
            case "List":
                ListConnections();
                break;
            case "Add":
                AddConnection();
                break;
            case "Delete":
                DeleteConnection();
                break;
            case "Back":
                return;
        }

        Execute(inn);
    }

    private static void ListConnections()
    {
        var connections = GetConnections();
        if (connections.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No saved connections found.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Name")
            .AddColumn("URL")
            .AddColumn("Database")
            .AddColumn("User");

        foreach (var connection in connections.OrderBy(connection => connection.SortOrder))
        {
            table.AddRow(
                Markup.Escape(connection.Name),
                Markup.Escape(connection.Url),
                Markup.Escape(connection.DB),
                Markup.Escape(connection.User));
        }

        AnsiConsole.Write(table);
    }

    private static void AddConnection()
    {
        var connections = new ArasConnections(InitSqLite.GetConnection());
        var setup = new SetupArasConnection();
        var innovator = setup.Run(connections);

        AnsiConsole.MarkupLine(innovator == null
            ? "[red]Connection was not saved because the connection test failed.[/]"
            : "[green]Connection saved successfully.[/]");
    }

    private static void DeleteConnection()
    {
        var connections = GetConnections();
        if (connections.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No saved connections found.[/]");
            return;
        }

        var selectedName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n Select a connection to delete:")
                .AddChoices(connections.OrderBy(connection => connection.SortOrder).Select(connection => connection.Name)));

        if (!AnsiConsole.Confirm($"Delete connection '{Markup.Escape(selectedName)}'?"))
        {
            return;
        }

        new ArasConnections(InitSqLite.GetConnection()).DeleteConnection(selectedName);
        AnsiConsole.MarkupLine("[green]Connection deleted.[/]");
    }

    private static List<ConnectionDTO> GetConnections()
        => new ArasConnections(InitSqLite.GetConnection()).GetConnections();
}