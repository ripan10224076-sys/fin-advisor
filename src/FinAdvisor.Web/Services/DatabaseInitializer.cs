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
        EnsureColumn(connection, "BusinessProfiles", "UserId", "INTEGER NULL");
        EnsureColumn(connection, "Consultations", "UserId", "INTEGER NULL");
        EnsureIndex(connection, "IX_Users_Email", "Users", "Email");
        EnsureIndex(connection, "IX_BusinessProfiles_UserId", "BusinessProfiles", "UserId");
        EnsureIndex(connection, "IX_Consultations_UserId", "Consultations", "UserId");
        NormalizeExistingUsers(connection);
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
                Explanation TEXT NOT NULL
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

        SeedRules(connection);
        SeedRecommendations(connection);
        SeedUsers(connection);
        SeedBusinessProfile(connection);
    }

    private static void SeedRules(SqliteConnection connection)
    {
        var count = Convert.ToInt32(Scalar(connection, "SELECT COUNT(*) FROM Rules"));
        if (count > 0)
        {
            return;
        }

        foreach (var rule in KnowledgeBase.GetDefaultRules())
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Rules (Code, Name, ConditionsJson, Recommendation, IsActive)
                VALUES ($code, $name, $conditions, $recommendation, 1)
                """;
            command.Parameters.AddWithValue("$code", rule.Code);
            command.Parameters.AddWithValue("$name", rule.Name);
            command.Parameters.AddWithValue("$conditions", JsonSerializer.Serialize(rule.Conditions));
            command.Parameters.AddWithValue("$recommendation", rule.Recommendation.ToString());
            command.ExecuteNonQuery();
        }
    }

    private static void SeedRecommendations(SqliteConnection connection)
    {
        var count = Convert.ToInt32(Scalar(connection, "SELECT COUNT(*) FROM Recommendations"));
        if (count > 0)
        {
            return;
        }

        foreach (var type in Enum.GetNames<FinAdvisor.Core.Models.RekomendasiJenis>())
        {
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Recommendations (Type, DetailsJson) VALUES ($type, $details)";
            command.Parameters.AddWithValue("$type", type);
            command.Parameters.AddWithValue("$details", JsonSerializer.Serialize(RecommendationCatalog.Get(Enum.Parse<FinAdvisor.Core.Models.RekomendasiJenis>(type))));
            command.ExecuteNonQuery();
        }
    }

    private void SeedUsers(SqliteConnection connection)
    {
        UpsertSeedUser(
            connection,
            "Administrator FIN-ADVISOR",
            "admin@finadvisor.local",
            "Admin12345!",
            AppRoles.Administrator,
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            forcePasswordReset: true);

        UpsertSeedUser(
            connection,
            "Pemilik UMKM",
            "umkm@finadvisor.local",
            "Umkm12345!",
            AppRoles.Customer,
            "Warung Maju Jaya",
            "Perdagangan",
            "Warung Sembako",
            "Alamat usaha belum diisi",
            "Tasikmalaya",
            "Jawa Barat",
            "08xxxxxxxxxx",
            forcePasswordReset: false);
    }

    private void UpsertSeedUser(
        SqliteConnection connection,
        string name,
        string email,
        string password,
        string role,
        string businessName,
        string businessCategory,
        string businessType,
        string address,
        string city,
        string province,
        string phone,
        bool forcePasswordReset)
    {
        using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT Id, PasswordHash FROM Users WHERE lower(Email) = lower($email) LIMIT 1";
        existsCommand.Parameters.AddWithValue("$email", email);
        using var reader = existsCommand.ExecuteReader();
        if (reader.Read())
        {
            var userId = reader.GetInt32(0);
            var passwordHash = reader.GetString(1);
            reader.Close();

            using var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = """
                UPDATE Users
                SET PasswordHash = $passwordHash,
                    Role = $role,
                    BusinessName = CASE WHEN $businessName = '' THEN BusinessName ELSE $businessName END,
                    BusinessCategory = CASE WHEN $businessCategory = '' THEN BusinessCategory ELSE $businessCategory END,
                    BusinessType = CASE WHEN $businessType = '' THEN BusinessType ELSE $businessType END,
                    Address = CASE WHEN $address = '' THEN Address ELSE $address END,
                    City = CASE WHEN $city = '' THEN City ELSE $city END,
                    Province = CASE WHEN $province = '' THEN Province ELSE $province END,
                    Phone = CASE WHEN $phone = '' THEN Phone ELSE $phone END,
                    IsActive = 1,
                    UpdatedAt = $updatedAt
                WHERE Id = $id
                """;
            updateCommand.Parameters.AddWithValue("$id", userId);
            updateCommand.Parameters.AddWithValue(
                "$passwordHash",
                forcePasswordReset || string.IsNullOrWhiteSpace(passwordHash) ? _passwordHasher.Hash(password) : passwordHash);
            updateCommand.Parameters.AddWithValue("$role", role);
            updateCommand.Parameters.AddWithValue("$businessName", businessName);
            updateCommand.Parameters.AddWithValue("$businessCategory", businessCategory);
            updateCommand.Parameters.AddWithValue("$businessType", businessType);
            updateCommand.Parameters.AddWithValue("$address", address);
            updateCommand.Parameters.AddWithValue("$city", city);
            updateCommand.Parameters.AddWithValue("$province", province);
            updateCommand.Parameters.AddWithValue("$phone", phone);
            updateCommand.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
            updateCommand.ExecuteNonQuery();

            return;
        }
        reader.Close();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users
                (Name, Email, Role, CreatedAt, PasswordHash, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, UpdatedAt)
            VALUES
                ($name, $email, $role, $createdAt, $passwordHash, $businessName, $businessCategory, $businessType, $address, $city, $province, $phone, 1, $updatedAt)
            """;
        command.Parameters.AddWithValue("$name", name);
        command.Parameters.AddWithValue("$email", email);
        command.Parameters.AddWithValue("$role", role);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$passwordHash", _passwordHasher.Hash(password));
        command.Parameters.AddWithValue("$businessName", businessName);
        command.Parameters.AddWithValue("$businessCategory", businessCategory);
        command.Parameters.AddWithValue("$businessType", businessType);
        command.Parameters.AddWithValue("$address", address);
        command.Parameters.AddWithValue("$city", city);
        command.Parameters.AddWithValue("$province", province);
        command.Parameters.AddWithValue("$phone", phone);
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static void SeedBusinessProfile(SqliteConnection connection)
    {
        var count = Convert.ToInt32(Scalar(connection, "SELECT COUNT(*) FROM BusinessProfiles"));
        if (count > 0)
        {
            return;
        }

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO BusinessProfiles
                (BusinessName, OwnerName, Phone, BusinessType, Address, MonthlyRevenue, FinancingPurpose, UpdatedAt, UserId)
            VALUES
                ('Warung Maju Jaya', 'Pemilik UMKM', '08xxxxxxxxxx', 'Perdagangan', 'Alamat usaha belum diisi', 'Belum diisi', 'Modal kerja', $updatedAt, (SELECT Id FROM Users WHERE lower(Email) = 'umkm@finadvisor.local' LIMIT 1))
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

    private static object Scalar(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteScalar() ?? 0;
    }

    private static void EnsureColumn(SqliteConnection connection, string tableName, string columnName, string definition)
    {
        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = $"PRAGMA table_info({tableName})";
        using var reader = checkCommand.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        reader.Close();
        Execute(connection, $"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition}");
    }

    private static void EnsureIndex(SqliteConnection connection, string indexName, string tableName, string columnName)
    {
        Execute(connection, $"CREATE INDEX IF NOT EXISTS {indexName} ON {tableName} ({columnName})");
    }

    private static void NormalizeExistingUsers(SqliteConnection connection)
    {
        Execute(connection, "UPDATE Users SET Role = 'Administrator' WHERE lower(Role) IN ('admin', 'administrator')");
        Execute(connection, "UPDATE Users SET Role = 'Customer' WHERE lower(Role) IN ('customer', 'umkm', 'umkm customer', 'pemilik umkm')");
        Execute(connection, "UPDATE Users SET UpdatedAt = CreatedAt WHERE UpdatedAt = ''");
    }
}
