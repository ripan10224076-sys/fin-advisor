using System.Text.Json;
using FinAdvisor.Core.KnowledgeBase;
using FinAdvisor.Core.Services;
using FinAdvisor.Web.Security;
using Microsoft.Data.Sqlite;

namespace FinAdvisor.Web.Services;

public class DatabaseInitializer
{
    private readonly SqliteConnectionFactory _factory;
    private readonly PasswordHasher _passwordHasher;

    public DatabaseInitializer(SqliteConnectionFactory factory, PasswordHasher passwordHasher)
    {
        _factory = factory;
        _passwordHasher = passwordHasher;
    }

    public void Initialize()
    {
        using var connection = _factory.Create();

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Role TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS BusinessProfiles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BusinessName TEXT NOT NULL,
                OwnerName TEXT NOT NULL,
                Phone TEXT NOT NULL,
                BusinessType TEXT NOT NULL,
                Address TEXT NOT NULL,
                MonthlyRevenue TEXT NOT NULL,
                FinancingPurpose TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Rules (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Code TEXT NOT NULL UNIQUE,
                Name TEXT NOT NULL,
                ConditionsJson TEXT NOT NULL,
                Recommendation TEXT NOT NULL,
                IsActive INTEGER NOT NULL DEFAULT 1
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Recommendations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Type TEXT NOT NULL UNIQUE,
                DetailsJson TEXT NOT NULL
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS Consultations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CreatedAt TEXT NOT NULL,
                BusinessName TEXT NOT NULL,
                InputJson TEXT NOT NULL,
                Recommendation TEXT NOT NULL,
                RuleCode TEXT NOT NULL,
                Score INTEGER NOT NULL,
                Explanation TEXT NOT NULL,
                UserId INTEGER NULL
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS History (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CreatedAt TEXT NOT NULL,
                Action TEXT NOT NULL,
                Description TEXT NOT NULL
            );
            """);

        // Users
        EnsureColumn(connection, "Users", "PasswordHash", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "BusinessName", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "BusinessCategory", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "BusinessType", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "Address", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "City", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "Province", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "Phone", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "Users", "IsActive", "INTEGER NOT NULL DEFAULT 1");
        EnsureColumn(connection, "Users", "UpdatedAt", "TEXT NOT NULL DEFAULT ''");

        // BusinessProfiles
        EnsureColumn(connection, "BusinessProfiles", "UserId", "INTEGER NULL");

        // Consultations
        EnsureColumn(connection, "Consultations", "UserId", "INTEGER NULL");

        // Indexes
        EnsureIndex(connection, "IX_Users_Email", "Users", "Email");
        EnsureIndex(connection, "IX_BusinessProfiles_UserId", "BusinessProfiles", "UserId");
        EnsureIndex(connection, "IX_Consultations_UserId", "Consultations", "UserId");

        NormalizeExistingUsers(connection);

        SeedRules(connection);
        SeedRecommendations(connection);
        SeedUsers(connection);
        SeedBusinessProfile(connection);
    }

    private void SeedRules(SqliteConnection connection)
    {
        var rules = KnowledgeBase.GetDefaultRules();
        foreach (var rule in rules)
        {
            var conditionsJson = JsonSerializer.Serialize(
                rule.Conditions.Select(c => new { c.FactName, c.ExpectedValue }).ToList());
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Rules (Code, Name, ConditionsJson, Recommendation, IsActive)
                VALUES ($code, $name, $conditions, $recommendation, 1)
                ON CONFLICT(Code) DO NOTHING
                """;
            command.Parameters.AddWithValue("$code", rule.Code);
            command.Parameters.AddWithValue("$name", rule.Name);
            command.Parameters.AddWithValue("$conditions", conditionsJson);
            command.Parameters.AddWithValue("$recommendation", rule.Recommendation.ToString());
            command.ExecuteNonQuery();
        }
    }

    private void SeedRecommendations(SqliteConnection connection)
    {
        var types = Enum.GetValues<FinAdvisor.Core.Models.RekomendasiJenis>();
        foreach (var type in types)
        {
            var details = RecommendationCatalog.Get(type);
            var detailsJson = JsonSerializer.Serialize(details);
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Recommendations (Type, DetailsJson)
                VALUES ($type, $details)
                ON CONFLICT(Type) DO UPDATE SET DetailsJson = excluded.DetailsJson
                """;
            command.Parameters.AddWithValue("$type", type.ToString());
            command.Parameters.AddWithValue("$details", detailsJson);
            command.ExecuteNonQuery();
        }
    }

    private void SeedUsers(SqliteConnection connection)
    {
        UpsertSeedUser(connection, "Admin", "admin@finadvisor.id", AppRoles.Administrator, "Admin@123");
    }

    private void UpsertSeedUser(SqliteConnection connection, string name, string email, string role, string password)
    {
        var existing = Scalar(connection, "SELECT COUNT(*) FROM Users WHERE lower(Email) = lower($email)",
            ("$email", email));
        if (Convert.ToInt64(existing) > 0)
        {
            return;
        }

        var passwordHash = _passwordHasher.Hash(password);
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users (Name, Email, Role, CreatedAt, PasswordHash, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, UpdatedAt)
            VALUES ($name, $email, $role, $createdAt, $passwordHash, '', '', '', '', '', '', '', 1, $updatedAt)
            """;
        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$email", email);
        command.Parameters.AddWithValue("$role", role);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static void SeedBusinessProfile(SqliteConnection connection)
    {
        var existing = Scalar(connection, "SELECT COUNT(*) FROM BusinessProfiles");
        if (Convert.ToInt64(existing) > 0)
        {
            return;
        }

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO BusinessProfiles (BusinessName, OwnerName, Phone, BusinessType, Address, MonthlyRevenue, FinancingPurpose, UpdatedAt)
            VALUES ('Contoh Usaha', 'Pemilik', '08123456789', 'Dagang', 'Jakarta', '10000000', 'Modal Kerja', $updatedAt)
            """;
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static void Execute(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static object? Scalar(SqliteConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return command.ExecuteScalar();
    }

    private static void EnsureColumn(SqliteConnection connection, string table, string column, string definition)
    {
        var exists = Scalar(connection,
            $"SELECT COUNT(*) FROM pragma_table_info('{table}') WHERE name = $col",
            ("$col", column));
        if (Convert.ToInt64(exists) > 0)
        {
            return;
        }

        Execute(connection, $"ALTER TABLE {table} ADD COLUMN {column} {definition}");
    }

    private static void EnsureIndex(SqliteConnection connection, string indexName, string table, string column)
    {
        var exists = Scalar(connection,
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'index' AND name = $name",
            ("$name", indexName));
        if (Convert.ToInt64(exists) > 0)
        {
            return;
        }

        Execute(connection, $"CREATE INDEX {indexName} ON {table}({column})");
    }

    private static void NormalizeExistingUsers(SqliteConnection connection)
    {
        Execute(connection, """
            UPDATE Users
            SET Email = lower(trim(Email))
            WHERE Email != lower(trim(Email))
            """);
    }
}