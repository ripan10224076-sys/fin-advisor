using FinAdvisor.Web.Models;
using FinAdvisor.Web.Security;

namespace FinAdvisor.Web.Services;

public class UserRepository
{
    private readonly SqliteConnectionFactory _factory;

    public UserRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public AppUser? GetByEmail(string email)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, Email, PasswordHash, Role, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, CreatedAt
            FROM Users
            WHERE lower(Email) = lower($email)
            LIMIT 1
            """;
        command.Parameters.AddWithValue("$email", email.Trim());
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public AppUser? GetById(int id)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, Email, PasswordHash, Role, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, CreatedAt
            FROM Users
            WHERE Id = $id
            LIMIT 1
            """;
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public List<AppUser> GetAllCustomers()
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, Email, PasswordHash, Role, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, CreatedAt
            FROM Users
            WHERE Role = $role
            ORDER BY Id DESC
            """;
        command.Parameters.AddWithValue("$role", AppRoles.Customer);
        using var reader = command.ExecuteReader();
        var users = new List<AppUser>();
        while (reader.Read())
        {
            users.Add(Map(reader));
        }

        return users;
    }

    public bool EmailExists(string email) => GetByEmail(email) is not null;

    public int CreateCustomer(RegisterInput input, string passwordHash)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Users
                (Name, Email, Role, CreatedAt, PasswordHash, BusinessName, BusinessCategory, BusinessType, Address, City, Province, Phone, IsActive, UpdatedAt)
            VALUES
                ($name, $email, $role, $createdAt, $passwordHash, $businessName, $businessCategory, $businessType, $address, $city, $province, $phone, 1, $updatedAt);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$name", input.FullName.Trim());
        command.Parameters.AddWithValue("$email", input.Email.Trim().ToLowerInvariant());
        command.Parameters.AddWithValue("$role", AppRoles.Customer);
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$passwordHash", passwordHash);
        command.Parameters.AddWithValue("$businessName", input.BusinessName.Trim());
        command.Parameters.AddWithValue("$businessCategory", input.BusinessCategory.Trim());
        command.Parameters.AddWithValue("$businessType", input.BusinessType.Trim());
        command.Parameters.AddWithValue("$address", input.Address.Trim());
        command.Parameters.AddWithValue("$city", input.City.Trim());
        command.Parameters.AddWithValue("$province", input.Province.Trim());
        command.Parameters.AddWithValue("$phone", input.Phone.Trim());
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void UpdateCustomerProfile(int userId, BusinessProfile profile)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Users
            SET Name = $ownerName,
                BusinessName = $businessName,
                BusinessType = $businessType,
                Address = $address,
                Phone = $phone,
                UpdatedAt = $updatedAt
            WHERE Id = $id AND Role = $role
            """;
        command.Parameters.AddWithValue("$id", userId);
        command.Parameters.AddWithValue("$role", AppRoles.Customer);
        command.Parameters.AddWithValue("$ownerName", profile.OwnerName.Trim());
        command.Parameters.AddWithValue("$businessName", profile.BusinessName.Trim());
        command.Parameters.AddWithValue("$businessType", profile.BusinessType.Trim());
        command.Parameters.AddWithValue("$address", profile.Address.Trim());
        command.Parameters.AddWithValue("$phone", profile.Phone.Trim());
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static AppUser Map(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        Email = reader.GetString(2),
        PasswordHash = reader.GetString(3),
        Role = reader.GetString(4),
        BusinessName = reader.GetString(5),
        BusinessCategory = reader.GetString(6),
        BusinessType = reader.GetString(7),
        Address = reader.GetString(8),
        City = reader.GetString(9),
        Province = reader.GetString(10),
        Phone = reader.GetString(11),
        IsActive = reader.GetInt32(12) == 1,
        CreatedAt = DateTime.Parse(reader.GetString(13)).ToLocalTime()
    };
}
