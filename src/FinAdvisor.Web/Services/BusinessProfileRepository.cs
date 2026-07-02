using FinAdvisor.Web.Models;

namespace FinAdvisor.Web.Services;

public class BusinessProfileRepository
{
    private readonly SqliteConnectionFactory _factory;

    public BusinessProfileRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public BusinessProfile Get(int? userId = null)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = userId.HasValue
            ? """
            SELECT Id, BusinessName, OwnerName, Phone, BusinessType, Address, MonthlyRevenue, FinancingPurpose, UpdatedAt
            FROM BusinessProfiles
            WHERE UserId = $userId
            ORDER BY Id
            LIMIT 1
            """
            : """
            SELECT Id, BusinessName, OwnerName, Phone, BusinessType, Address, MonthlyRevenue, FinancingPurpose, UpdatedAt
            FROM BusinessProfiles
            ORDER BY Id
            LIMIT 1
            """;
        if (userId.HasValue)
        {
            command.Parameters.AddWithValue("$userId", userId.Value);
        }

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return new BusinessProfile();
        }

        return new BusinessProfile
        {
            Id = reader.GetInt32(0),
            BusinessName = reader.GetString(1),
            OwnerName = reader.GetString(2),
            Phone = reader.GetString(3),
            BusinessType = reader.GetString(4),
            Address = reader.GetString(5),
            MonthlyRevenue = reader.GetString(6),
            FinancingPurpose = reader.GetString(7),
            UpdatedAt = DateTime.Parse(reader.GetString(8)).ToLocalTime()
        };
    }

    public void Save(BusinessProfile profile, int? userId = null)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();

        if (profile.Id == 0)
        {
            command.CommandText = """
                INSERT INTO BusinessProfiles
                    (BusinessName, OwnerName, Phone, BusinessType, Address, MonthlyRevenue, FinancingPurpose, UpdatedAt, UserId)
                VALUES
                    ($businessName, $ownerName, $phone, $businessType, $address, $monthlyRevenue, $financingPurpose, $updatedAt, $userId)
                """;
            command.Parameters.AddWithValue("$userId", userId.HasValue ? userId.Value : DBNull.Value);
        }
        else
        {
            command.CommandText = """
                UPDATE BusinessProfiles
                SET BusinessName = $businessName,
                    OwnerName = $ownerName,
                    Phone = $phone,
                    BusinessType = $businessType,
                    Address = $address,
                    MonthlyRevenue = $monthlyRevenue,
                    FinancingPurpose = $financingPurpose,
                    UpdatedAt = $updatedAt
                WHERE Id = $id
                """;
            command.Parameters.AddWithValue("$id", profile.Id);
        }

        command.Parameters.AddWithValue("$businessName", profile.BusinessName.Trim());
        command.Parameters.AddWithValue("$ownerName", profile.OwnerName.Trim());
        command.Parameters.AddWithValue("$phone", profile.Phone.Trim());
        command.Parameters.AddWithValue("$businessType", profile.BusinessType.Trim());
        command.Parameters.AddWithValue("$address", profile.Address.Trim());
        command.Parameters.AddWithValue("$monthlyRevenue", profile.MonthlyRevenue?.Trim() ?? "");
        command.Parameters.AddWithValue("$financingPurpose", profile.FinancingPurpose?.Trim() ?? "");
        command.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));
        command.ExecuteNonQuery();
    }
}
