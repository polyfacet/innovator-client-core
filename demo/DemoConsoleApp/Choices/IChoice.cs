namespace DemoConsoleApp.Choices;
public interface IChoice
{
    string Name { get; }
    void Execute(Innovator.Client.IOM.Innovator inn);
}