using SQLite;

namespace PortfolioApi
{

    public static class ConnectionStringHelper
    {
        public static (string DbPath, string? Password) Parse(string connectionString)
        {
            // Split ADO.NET "key=value;key=value"
            var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            string? dbPath = null;
            string? password = null;

            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;

                var key = kv[0].Trim().ToLowerInvariant();
                var value = kv[1].Trim();

                if (key == "data source")
                    dbPath = value;
                else if (key == "password")
                    password = value;
            }

            if (dbPath == null)
                throw new ArgumentException("Connection string must contain 'Data Source='");

            // Normalize path
            if (!Path.IsPathRooted(dbPath))
                dbPath = Path.Combine(Directory.GetCurrentDirectory(), dbPath);

            return (dbPath, password);
        }
    }
    public static class DbChecker
    {
        public static void EnsureDatabaseExists(string connectionString)
        {
            try
            {
                var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "portfolio.db");

                // Parse connection string to get DB path and password
                var builder = new SQLiteConnectionString(
                    databasePath: dbPath,
                    storeDateTimeAsTicks: true,
                    key: "TestPasscode" 
                );

                // Ensure directory exists
                var dbFolder = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(dbFolder) && !Directory.Exists(dbFolder))
                    Directory.CreateDirectory(dbFolder);

                // Check if database file exists
                var isNew = !File.Exists(dbPath);

                // Establish connection (this will create the file if it doesn't exist)
                using var connection = new SQLiteConnection(builder);

                if (isNew)
                {
                    Console.WriteLine("Database non trovato, creazione in corso...");

                    var createTablesSql = @"
                        CREATE TABLE Users (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Email TEXT NOT NULL UNIQUE,
                            Username TEXT NOT NULL,
                            PasswordHash TEXT NOT NULL,
                            AccessToken TEXT,
                            AccessTokenExpiryTime DATETIME,
                            RefreshToken TEXT,
                            RefreshTokenExpiryTime DATETIME
                        );
                    ";

                    connection.Execute(createTablesSql);

                    Console.WriteLine("Schema created succesfully");
                }
                else
                {
                    Console.WriteLine("Schema not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while verifying/creating the schema: {ex.Message}");
                throw;
            }
        }
    }
}
