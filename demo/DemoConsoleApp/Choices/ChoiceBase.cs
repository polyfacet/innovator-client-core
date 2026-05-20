namespace DemoConsoleApp.Choices;

public abstract class ChoiceBase : IChoice
{
    public abstract string Name { get; }
    public abstract void Execute(Innovator.Client.IOM.Innovator inn);

    protected void LogLine(string markup, bool includeTimestamp = true)
        => ConsoleLog.Line(markup, includeTimestamp);
}