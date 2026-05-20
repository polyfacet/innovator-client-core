public class SessionDTO(string url, string database, string username, string password)
{
    public string Url { get; set; } = url;
    public string Database { get; set; } = database;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
}