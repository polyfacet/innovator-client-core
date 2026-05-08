public class DefaultEnvConfig
{
    public const string DefaultUserLabel = "admin";
    public static Innovator.Client.IOM.Innovator GetAdminSession()
    {
        SessionDTO session = SessionConfig.GetSessionConfig(DefaultUserLabel);
        return Connections.Session.CreateSession(session.Url, session.Database, session.Username, session.Password);
    }


}