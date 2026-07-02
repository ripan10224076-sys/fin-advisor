using FinAdvisor.Web.Models;
using FinAdvisor.Web.Security;

namespace FinAdvisor.Web.Services;

public class AdminDashboardService
{
    private readonly SqliteConnectionFactory _factory;

    public AdminDashboardService(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public AdminDashboardStats GetStats()
    {
        using var connection = _factory.Create();
        return new AdminDashboardStats
        {
            TotalUsers = Count(connection, "SELECT COUNT(*) FROM Users"),
            TotalCustomers = Count(connection, "SELECT COUNT(*) FROM Users WHERE Role = $role", ("$role", AppRoles.Customer)),
            TotalConsultations = Count(connection, "SELECT COUNT(*) FROM Consultations"),
            TotalRules = Count(connection, "SELECT COUNT(*) FROM Rules"),
            ActiveRules = Count(connection, "SELECT COUNT(*) FROM Rules WHERE IsActive = 1"),
            TodayConsultations = Count(connection, "SELECT COUNT(*) FROM Consultations WHERE date(CreatedAt) = date('now')")
        };
    }

    public List<ConsultationRecord> GetRecentConsultations(int take = 6)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, CreatedAt, BusinessName, InputJson, Recommendation, RuleCode, Score, Explanation
            FROM Consultations
            ORDER BY Id DESC
            LIMIT $take
            """;
        command.Parameters.AddWithValue("$take", take);
        using var reader = command.ExecuteReader();
        var items = new List<ConsultationRecord>();
        while (reader.Read())
        {
            items.Add(new ConsultationRecord
            {
                Id = reader.GetInt32(0),
                CreatedAt = DateTime.Parse(reader.GetString(1)).ToLocalTime(),
                BusinessName = reader.GetString(2),
                InputJson = reader.GetString(3),
                Recommendation = reader.GetString(4),
                RuleCode = reader.GetString(5),
                Score = reader.GetInt32(6),
                Explanation = reader.GetString(7)
            });
        }

        return items;
    }

    private static int Count(Microsoft.Data.Sqlite.SqliteConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return Convert.ToInt32(command.ExecuteScalar());
    }
}
