using Users;

namespace DemoConsoleApp.Choices;

public static class ErrorHandler
{
    public static bool HandleError(Item item, string action)
    {
        if (!item.isError())
        {
            return false;
        }

        AnsiConsole.MarkupLine($"[red]Error {action}: {item.getErrorString()}[/]");
        return true;
    }
}
