using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Storage.SQLite;

public static class InitSqLite
{
    private const string ConnectionStringName = "SqliteConnection";
    private static string _connectionString = string.Empty;

    public static string ConnectionString
    {
        get
        {
            if (string.IsNullOrEmpty(_connectionString)) throw new InvalidOperationException("SQLite connection string has not been initialized.");
            return _connectionString;
        }
    }

    public static void Initialize(IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var connectionStringTemplate = configuration.GetConnectionString(ConnectionStringName) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(connectionStringTemplate)) throw new InvalidOperationException($"Missing '{ConnectionStringName}' connection string in configuration.");

        // Resolve the database path to the app directory
        var databasePath = Path.Combine(AppContext.BaseDirectory, "demo.db");
        _connectionString = $"Data Source={databasePath}";

        CreateDatabaseIfNeeded();
        LogInitialization();
    }

    private static void CreateDatabaseIfNeeded()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS InitializationLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CreatedAt TEXT NOT NULL,
                    Message TEXT NOT NULL
                );";
            command.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException("Failed to initialize SQLite database.", ex);
        }
    }

    private static void LogInitialization()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO InitializationLog (CreatedAt, Message)
                VALUES (@createdAt, @message);";
            command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("O"));
            command.Parameters.AddWithValue("@message", "Database initialized successfully.");
            command.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            throw new InvalidOperationException("Failed to log initialization.", ex);
        }
    }

    private static SqliteConnection? _connection;

    public static SqliteConnection GetConnection()
    {
        if (_connection == null)
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();
        }
        else if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }

        return _connection;
    }

    public static void DisposeConnection()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}
