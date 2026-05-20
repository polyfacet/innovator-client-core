
using Users;

namespace DemoConsoleApp.Choices;

public class CreateDeleteUser : ChoiceBase
{
    public override string Name => "Create/Delete User";

    public override void Execute(Innovator.Client.IOM.Innovator inn)
    {
        string loginName = Generators.ShortGUID();
        LogLine($"Creating user with loginName: [blue]{loginName}[/]...");
        Item user = new User(inn).CreateUser(loginName, "test", loginName, "Doe");
        if (ErrorHandler.HandleError(user, "creating user")) return;

        LogLine($"User created with ID: [green]{user.getID()}[/]");

        if (!AnsiConsole.Confirm($"Delete user [red]{user.getID()}[/]?"))
        {
            LogLine("[yellow]Delete cancelled.[/]");
            return;
        }

        LogLine($"Deleting user with ID: [red]{user.getID()}[/]...");
        user.apply("delete");
        if (ErrorHandler.HandleError(user, "deleting user")) return;
        LogLine($"User with ID: [green]{user.getID()}[/] deleted successfully.");       
    }
}
