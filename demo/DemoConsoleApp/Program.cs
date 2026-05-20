using DemoConsoleApp.Choices;
using Microsoft.Extensions.Configuration;

var config = Config.GetConfig();
var settings = new AppSettings();
config.GetSection("AppSettings").Bind(settings);

string appName = settings.ApplicationName;
AnsiConsole.Write(new FigletText(appName).Centered().Color(Color.Blue));

// Connect to Aras using connection string from appsettings.json
string connectionString = config.GetConnectionString("DefaultConnection") ?? String.Empty;

SessionDTO session = ConvertArasConnectionStringToSessionDTO(connectionString);
AnsiConsole.MarkupLine($"Preparing to connect to [green]{session.Url} : {session.Database}[/] as user [green]{session.Username}[/]...");

Innovator.Client.IOM.Innovator? inn = null;
AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .SpinnerStyle(Style.Parse("yellow"))
    .Start("Connecting to Innovator...", ctx =>
    {
        inn = SessionManager.CreateSession("admin", session.Url, session.Database, session.Username, session.Password);
    });

if (inn == null)
{
    AnsiConsole.MarkupLine("[red]Failed to connect to Innovator. Please check your connection string and try again.[/]");
    return;
}
   
AnsiConsole.MarkupLine("[green]Connected successfully![/]");

Dictionary<string, IChoice> choicesDict = new Dictionary<string, IChoice>();
foreach (var choice in ChoicesFactory.GetChoices())
{
    choicesDict[choice.Name] = choice;
}


// 2. Skapa en interaktiv flervalsmeny
while (true) {
    var val = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("What do you want to do?")
        .PageSize(10)
        .AddChoices(choicesDict.Keys));

    AnsiConsole.MarkupLine($"You selected: [green]{val}[/]");
    choicesDict[val].Execute(inn);
}


//// 3. Rendera en vacker tabell
//var table = new Table();
//table.AddColumn("ID");
//table.AddColumn(new TableColumn("Namn").Centered());

//table.AddRow("1", "Apples");
//table.AddRow("2", "[green]Bananas[/]");

//AnsiConsole.Write(table);


SessionDTO ConvertArasConnectionStringToSessionDTO(string connectionString)
{
    var parameters = connectionString.Split(';')
        .Select(part => part.Split('='))
        .Where(part => part.Length == 2)
        .ToDictionary(part => part[0].Trim(), part => part[1].Trim());

    string url = parameters.ContainsKey("Url") ? parameters["Url"] : throw new Exception("URL not found in connection string.");
    string database = parameters.ContainsKey("Database") ? parameters["Database"] : throw new Exception("Database not found in connection string.");
    string username = parameters.ContainsKey("User") ? parameters["User"] : throw new Exception("Username not found in connection string.");
    string password = parameters.ContainsKey("Password") ? parameters["Password"] : throw new Exception("Password not found in connection string.");

    return new SessionDTO(url, database, username, password);
}