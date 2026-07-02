using System.Text.Json;
using FinAdvisor.Core.Models;
using FinAdvisor.Web.Models;

namespace FinAdvisor.Web.Services;

public class ConsultationRepository
{
    private readonly SqliteConnectionFactory _factory;

    public ConsultationRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public int Add(string businessName, UmkmProfile input, HasilRekomendasi result, int? userId = null)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Consultations (CreatedAt, BusinessName, InputJson, Recommendation, RuleCode, Score, Explanation, UserId)
            VALUES ($createdAt, $businessName, $input, $recommendation, $ruleCode, $score, $explanation, $userId);
            SELECT last_insert_rowid();
            """;
        command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$businessName", string.IsNullOrWhiteSpace(businessName) ? "UMKM Tanpa Nama" : businessName.Trim());
        command.Parameters.AddWithValue("$input", JsonSerializer.Serialize(input));
        command.Parameters.AddWithValue("$recommendation", result.Jenis.ToString());
        command.Parameters.AddWithValue("$ruleCode", result.KodeAturan);
        command.Parameters.AddWithValue("$score", result.SkorKesesuaian);
        command.Parameters.AddWithValue("$explanation", result.Penjelasan);
        command.Parameters.AddWithValue("$userId", userId.HasValue ? userId.Value : DBNull.Value);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<ConsultationRecord> Search(string? query, string? recommendation, int? userId = null)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, CreatedAt, BusinessName, InputJson, Recommendation, RuleCode, Score, Explanation
            FROM Consultations
            WHERE ($query = '' OR BusinessName LIKE '%' || $query || '%' OR Recommendation LIKE '%' || $query || '%')
              AND ($recommendation = '' OR Recommendation = $recommendation)
              AND ($userId IS NULL OR UserId = $userId)
            ORDER BY Id DESC
            """;
        command.Parameters.AddWithValue("$query", query ?? "");
        command.Parameters.AddWithValue("$recommendation", recommendation ?? "");
        command.Parameters.AddWithValue("$userId", userId.HasValue ? userId.Value : DBNull.Value);
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
}
