using Microsoft.Data.Sqlite;

public class ArasConnections
{
    SqliteConnection _connection;
    public ArasConnections(SqliteConnection connection)
    {
        _connection = connection;
        Init();
    }

    private void Init()
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ArasConnections (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Url TEXT NOT NULL,
                DB TEXT NOT NULL,
                User TEXT NOT NULL,
                Password TEXT NOT NULL,
                SortOrder INTEGER NOT NULL DEFAULT 0
            );";
        cmd.ExecuteNonQuery();
    }

    public void SaveConnection(ConnectionDTO connection)
    {
        int sortOrder;
        using (var sortCmd = _connection.CreateCommand())
        {
            sortCmd.CommandText = "SELECT COALESCE(MAX(SortOrder), 0) FROM ArasConnections";
            sortOrder = Convert.ToInt32(sortCmd.ExecuteScalar() ?? 0) + 1;
        }

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "INSERT INTO ArasConnections (Name, Url, DB, User, Password, SortOrder) VALUES (@Name, @Url, @DB, @User, @Password, @SortOrder)";
        cmd.Parameters.AddWithValue("@Name", connection.Name);
        cmd.Parameters.AddWithValue("@Url", connection.Url);
        cmd.Parameters.AddWithValue("@DB", connection.DB);
        cmd.Parameters.AddWithValue("@User", connection.User);
        cmd.Parameters.AddWithValue("@Password", connection.Password);
        cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
        cmd.ExecuteNonQuery();
    }

    public List<ConnectionDTO> GetConnections()
    {
        var connections = new List<ConnectionDTO>();

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "SELECT Name, Url, DB, User, Password, SortOrder FROM ArasConnections";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            connections.Add(new ConnectionDTO
            {
                Name = reader.GetString(0),
                Url = reader.GetString(1),
                DB = reader.GetString(2),
                User = reader.GetString(3),
                Password = reader.GetString(4),
                SortOrder = reader.GetInt32(5)
            });
        }

        return connections;
    }

    public void DeleteConnection(string name)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "DELETE FROM ArasConnections WHERE Name = @Name";
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.ExecuteNonQuery();
    }
}