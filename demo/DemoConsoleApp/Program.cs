using DemoConsoleApp.Choices;
using Microsoft.Extensions.Configuration;

// Set up configuration to read from appsettings.json
var config = Config.GetConfig();
var settings = new AppSettings();
config.GetSection("AppSettings").Bind(settings);

string appName = settings.ApplicationName;
// Write fancy ASCII art title using FigletText from Spectre.Console
AnsiConsole.Write(new FigletText(appName).Centered().Color(Color.Blue));

// Connect to Aras using connection string from appsettings.json
string connectionString = config.GetConnectionString("DefaultConnection") ?? String.Empty;

Innovator.Client.IOM.Innovator? inn = InnovatorSession.CreateInnovatorSession(connectionString);
if (inn == null)
{
    AnsiConsole.MarkupLine("[red]Failed to connect to Innovator. Please check your connection string and try again.[/]");
    return;
} 
AnsiConsole.MarkupLine("[green]Connected successfully![/]");

// Get choices dictionary  
Dictionary<string, IChoice> choicesDict = ChoicesFactory.GetChoicesDictionary();

// Run the selection menu
while (true) RunSelectionMenu(inn, choicesDict, settings.MaxItemsPerPage);


static void RunSelectionMenu(Innovator.Client.IOM.Innovator inn, IDictionary<string, IChoice> choicesDict, int maxItemsPerPage = 10)
{
    var val = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What do you want to do?")
            .PageSize(maxItemsPerPage)
            .AddChoices(choicesDict.Keys));

    AnsiConsole.MarkupLine($"You selected: [green]{val}[/]");
    choicesDict[val].Execute(inn);
}
