using DemoConsoleApp.Choices;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

// Set up configuration to read from appsettings.json
var config = Config.GetConfig();
Storage.SQLite.InitSqLite.Initialize(config);
var settings = new AppSettings();
config.GetSection("AppSettings").Bind(settings);

string appName = settings.ApplicationName;
// Write fancy ASCII art title using FigletText from Spectre.Console
AnsiConsole.Write(new FigletText(appName).Centered().Color(Color.Blue));

Innovator.Client.IOM.Innovator? inn = null;
inn = GetDefaultConnectionAndConnect();
if (inn == null)
{
    AnsiConsole.MarkupLine("[red]Unable to connect to Innovator. Exiting application.[/]");
    return;
}

// Get choices dictionary  
Dictionary<string, IChoice> choicesDict = ChoicesFactory.GetChoicesDictionary();

// Run the selection menu
while (true) inn = RunSelectionMenu(inn, choicesDict, settings.MaxItemsPerPage);


static Innovator.Client.IOM.Innovator RunSelectionMenu(Innovator.Client.IOM.Innovator inn, IDictionary<string, IChoice> choicesDict, int maxItemsPerPage = 10)
{
    var val = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\n What do you want to do?")
            .PageSize(maxItemsPerPage)
            .AddChoices(choicesDict.Keys));

    AnsiConsole.MarkupLine($"You selected: [green]{val}[/]");
    var choice = choicesDict[val];
    choice.Execute(inn);

    return choice switch
    {
        ArasConnectionsChoice connectionsChoice when connectionsChoice.ConnectedInnovator is not null
            => connectionsChoice.ConnectedInnovator,
        _ => inn
    };
}

Innovator.Client.IOM.Innovator? GetDefaultConnectionAndConnect()
{
    Innovator.Client.IOM.Innovator? inn = null;
    var connection = Storage.SQLite.InitSqLite.GetConnection();
    ArasConnections arasConnections = new ArasConnections(connection);
    if (arasConnections.GetConnections().Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]No saved connections found. Please add a connection to get started.[/]");
        inn = RunArasConnectionSetup(arasConnections);
        if (inn == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to connect to Innovator. Please check your connection string and try again.[/]");
            return null;
        }
        AnsiConsole.MarkupLine("[green]Connected successfully![/]");
        return inn;
    }

    if (arasConnections.GetConnections().Count > 0)
    {
        // Get the connection with lowest SortOrder
        var defaultConnection = arasConnections.GetConnections().OrderBy(c => c.SortOrder).First();
        AnsiConsole.MarkupLine($"[green]Using default connection: {defaultConnection.Name}[/]");
        inn = InnovatorSession.CreateInnovatorSession(defaultConnection);
        if (inn == null)
        {
            AnsiConsole.MarkupLine("[red]Failed to connect to Innovator. Please check your connection string and try again.[/]");
            return null;
        } 
        AnsiConsole.MarkupLine("[green]Connected successfully![/]");
    }
    return inn;
}

Innovator.Client.IOM.Innovator? RunArasConnectionSetup(ArasConnections arasConnections)
{
    SetupArasConnection setup = new SetupArasConnection();
    return setup.Run(arasConnections);   
}