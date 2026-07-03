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