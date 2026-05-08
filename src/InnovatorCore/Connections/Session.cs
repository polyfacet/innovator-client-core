
using Innovator.Client;

namespace Connections;

public class Session
{
     private const string CONNECTION_NAME = "TestConnection";
     public int? TimeOutMilliSecs = 100000;

     private string Url;
     private string DB;
     private string UserName;
     private string Password;

     public Session(string url, string database, string username, string password)
     {
        Url = url;
        DB = database;
        UserName = username;
        Password = password;
     }
    
    public static Innovator.Client.IOM.Innovator CreateSession(string url, string database, string username, string password)
    {
        Session session = new Session(url, database, username, password);
        return session.GetInnovator();        
    }

    private Innovator.Client.IOM.Innovator GetInnovator() {
      Innovator.Client.IOM.Innovator inn;
            IRemoteConnection conn;
            ConnectionPreferences connectionPreferences = new ConnectionPreferences();
            connectionPreferences.Url = Url;
            connectionPreferences.Name = CONNECTION_NAME;
            connectionPreferences.DefaultTimeout = TimeOutMilliSecs;
            conn = Innovator.Client.Factory.GetConnection(connectionPreferences);

            ICredentials credentials = new ExplicitCredentials(DB, UserName, Password);
            if (IsMD5(Password)) credentials = new ExplicitHashCredentials(DB, UserName, Password);
            conn.Login(credentials);
            inn = new Innovator.Client.IOM.Innovator(conn);
            
            return inn;
        }

        public static bool IsMD5(string input)
        {
            if (String.IsNullOrEmpty(input)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(input, "^[0-9a-fA-F]{32}$");
        }
}