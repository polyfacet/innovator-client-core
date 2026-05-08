using System.Xml;

public class SessionConfig
{

    const string DefaultConfigFile = "TestFixture.config";

    
    public static SessionDTO GetSessionConfig(string userLabel, string environmentName = "" , string configFile = DefaultConfigFile)
    {
        if (!File.Exists(configFile)) throw new FileNotFoundException($"The configuration file '{configFile}' was not found.");

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(configFile);

        XmlNode? environment = xmlDoc.GetElementsByTagName("Environment")[0];
        if (environmentName != "")
        {
             environment = xmlDoc.SelectSingleNode($"//Environment[@name='{environmentName}']");
        }
        if (environment == null) throw new Exception($"Environment '{environmentName}' not found in configuration file.");

        var urlNode = environment.SelectSingleNode("Url");
        if (urlNode == null) throw new Exception($"Url node not found in environment '{environmentName}'.");
        string url = urlNode.InnerText;
        var databaseNode = environment.SelectSingleNode("DatabaseName");
        if (databaseNode == null) throw new Exception($"Database node not found in environment '{environmentName}'.");
        string database = databaseNode.InnerText;

        var userNode = environment.SelectSingleNode($"//User[@label='{userLabel}']");
        if (userNode == null) throw new Exception($"User with label '{userLabel}' not found in environment '{environmentName}'.");

        string username = userNode.SelectSingleNode("Login")?.InnerText ?? string.Empty;
        string password = userNode.SelectSingleNode("Password")?.InnerText ?? string.Empty;
        
        SessionDTO sessionDTO = new SessionDTO(url, database, username, password);
        
        return sessionDTO;
    }
    


}