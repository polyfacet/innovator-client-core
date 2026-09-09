using Spectre.Console;
using Storage.SQLite;

namespace DemoConsoleApp.Choices;

public class ArasConnectionsChoice : ChoiceBase
{
    public override string Name => "Aras Connections";

    public Innovator.Client.IOM.Innovator? ConnectedInnovator { get; private set; }
    private ConnectionDTO? CurrentConnection { get; set; }

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        ConnectedInnovator = null;

        CurrentConnection ??= GetConnections()
            .OrderBy(connection => connection.SortOrder)
            .FirstOrDefault();

        while (true)
        {
            var actions = new[] { "Switch", "Show Current Connection Info", "List", "Add", "Delete", "Back" };
            var selectedAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\n Select connection action:")
                    .EnableSearch()
                    .AddChoices(actions));

            switch (selectedAction)
            {
                case "List":
                    ListConnections();
                    break;
                case "Show Current Connection Info":
                    ShowCurrentConnectionInfo();
                    break;
                case "Add":
                    AddConnection();
                    break;
                case "Switch":
                    var switchConnection = new SwitchArasConnection();
                    switchConnection.Execute(inn);
                    if (switchConnection.ConnectedInnovator is not null)
                    {
                        ConnectedInnovator = switchConnection.ConnectedInnovator;
                        CurrentConnection = GetConnections().First(connection => connection.Name == switchConnection.ConnectedConnectionName);
                    }
                    break;
                case "Delete":
                    DeleteConnection();
                    break;
                case "Back":
                    return;
            }
        }
    }

    private void ShowCurrentConnectionInfo()
    {
        if (CurrentConnection is null)
        {
            AnsiConsole.MarkupLine("[yellow]No current connection information is available.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Name", Markup.Escape(CurrentConnection.Name));
        table.AddRow("URL", Markup.Escape(CurrentConnection.Url));
        table.AddRow("Database", Markup.Escape(CurrentConnection.DB));
        table.AddRow("User", Markup.Escape(CurrentConnection.User));

        AnsiConsole.Write(table);
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
                .EnableSearch()
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