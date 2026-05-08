using Connections;

public class SessionManager
{

    private static Dictionary<string, Innovator.Client.IOM.Innovator> sessions = new Dictionary<string, Innovator.Client.IOM.Innovator>();

    public static Innovator.Client.IOM.Innovator CreateSession(string name, string url, string database, string username, string password)
    {
        if (sessions.ContainsKey(name))
        {
            return sessions[name];
        }
        var session = Session.CreateSession(url, database, username, password);
        sessions[name] = session;
        return session;
    }

    public static Innovator.Client.IOM.Innovator GetSession(string name)
    {
        if (sessions.ContainsKey(name))
        {
            return sessions[name];
        }
        throw new Exception($"Session with name {name} does not exist.");
    }

}