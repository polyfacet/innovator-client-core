using Spectre.Console;

public class SetupArasConnection
{
    internal Innovator.Client.IOM.Innovator? Run(ArasConnections arasConnections)
    {
        ConnectionDTO connection = new ConnectionDTO();
        connection.Name = AnsiConsole.Ask<string>("Enter a name for the connection:");
        connection.Url = AnsiConsole.Ask<string>("Enter the Aras Innovator URL:");
        connection.DB = AnsiConsole.Ask<string>("Enter the database name:");
        connection.User = AnsiConsole.Ask<string>("Enter the username:");
        connection.Password = AnsiConsole.Prompt(new TextPrompt<string>("Enter the password:").PromptStyle("red").Secret());

        Innovator.Client.IOM.Innovator? inn = InnovatorSession.CreateInnovatorSession(connection);
        if (inn != null)
        {
            arasConnections.SaveConnection(connection);    
        }
        return inn;
    }
}