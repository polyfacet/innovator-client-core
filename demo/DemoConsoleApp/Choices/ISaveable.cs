namespace DemoConsoleApp.Choices;

/// <summary>
/// Interface for choices that need access to AppSettings for storing/retrieving configuration
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// Initialize the choice with AppSettings to enable storing and retrieving settings
    /// </summary>
    void InitializeSettings(AppSettings appSettings);
}
