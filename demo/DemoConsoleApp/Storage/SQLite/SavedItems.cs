using Microsoft.Data.Sqlite;

namespace Storage.SQLite;

public sealed class SavedItems
{
    private readonly SqliteConnection _connection;

    public SavedItems(SqliteConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        Initialize();
    }

    private void Initialize()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS SavedItems (
                ItemType TEXT NOT NULL,
                Name TEXT NOT NULL,
                ConfigId TEXT NOT NULL,
                SavedDateTime TEXT NOT NULL,
                PRIMARY KEY (ItemType, Name)
            );";
        command.ExecuteNonQuery();
    }

    public void Save(string itemType, string name, string configId)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO SavedItems (ItemType, Name, ConfigId, SavedDateTime)
            VALUES (@itemType, @name, @configId, @savedDateTime)
            ON CONFLICT (ItemType, Name) DO UPDATE SET
                ConfigId = excluded.ConfigId,
                SavedDateTime = excluded.SavedDateTime;";
        command.Parameters.AddWithValue("@itemType", itemType);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@configId", configId);
        command.Parameters.AddWithValue("@savedDateTime", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    public List<string> GetItemTypes()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT DISTINCT ItemType FROM SavedItems ORDER BY ItemType";

        using var reader = command.ExecuteReader();
        var itemTypes = new List<string>();
        while (reader.Read()) itemTypes.Add(reader.GetString(0));
        return itemTypes;
    }

    public List<SavedItem> GetItems(string? itemType = null)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT ItemType, Name, ConfigId, SavedDateTime
            FROM SavedItems
            WHERE @itemType IS NULL OR ItemType = @itemType
            ORDER BY ItemType, SavedDateTime DESC, Name;";
        command.Parameters.AddWithValue("@itemType", (object?)itemType ?? DBNull.Value);

        using var reader = command.ExecuteReader();
        var items = new List<SavedItem>();
        while (reader.Read())
        {
            items.Add(new SavedItem
            {
                ItemType = reader.GetString(0),
                Name = reader.GetString(1),
                ConfigId = reader.GetString(2),
                SavedDateTime = DateTime.Parse(reader.GetString(3))
            });
        }

        return items;
    }

    public void Delete(string itemType, string name)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM SavedItems WHERE ItemType = @itemType AND Name = @name";
        command.Parameters.AddWithValue("@itemType", itemType);
        command.Parameters.AddWithValue("@name", name);
        command.ExecuteNonQuery();
    }
}

public sealed class SavedItem
{
    public string ItemType { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string ConfigId { get; init; } = string.Empty;
    public DateTime SavedDateTime { get; init; }
}
