public class InnovatorSession
{
    public static Innovator.Client.IOM.Innovator? CreateInnovatorSession(string connectionString, string connectionName = "admin")
    {
        SessionDTO session = ConvertArasConnectionStringToSessionDTO(connectionString);
        AnsiConsole.MarkupLine($"Preparing to connect to [green]{session.Url} : {session.Database}[/] as user [green]{session.Username}[/]...");
        Innovator.Client.IOM.Innovator? inn = null;

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("yellow"))
            .Start("Connecting to Innovator...", ctx =>
            {
                inn = SessionManager.CreateSession(connectionName, session.Url, session.Database, session.Username, session.Password);
            });

        return inn;
    }
    private static SessionDTO ConvertArasConnectionStringToSessionDTO(string connectionString)
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
}