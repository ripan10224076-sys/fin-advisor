using System.Text.Json;
using FinAdvisor.Core.Models;
using FinAdvisor.Core.ExpertSystem;
using FinAdvisor.Web.Models;

namespace FinAdvisor.Web.Services;

public class RuleRepository
{
    private readonly SqliteConnectionFactory _factory;

    public RuleRepository(SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public List<RuleRecord> GetAll()
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Code, Name, ConditionsJson, Recommendation, IsActive FROM Rules ORDER BY Code";
        using var reader = command.ExecuteReader();
        var rules = new List<RuleRecord>();
        while (reader.Read())
        {
            rules.Add(new RuleRecord
            {
                Id = reader.GetInt32(0),
                Code = reader.GetString(1),
                Name = reader.GetString(2),
                ConditionsJson = reader.GetString(3),
                Recommendation = Enum.Parse<RekomendasiJenis>(reader.GetString(4)),
                IsActive = reader.GetInt32(5) == 1
            });
        }

        return rules;
    }

    public List<Rule> GetActiveDomainRules() => GetAll()
        .Where(rule => rule.IsActive)
        .Select(ToDomainRule)
        .ToList();

    public RuleRecord? GetById(int id) => GetAll().FirstOrDefault(rule => rule.Id == id);

    public void Save(RuleRecord record)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        if (record.Id == 0)
        {
            command.CommandText = """
                INSERT INTO Rules (Code, Name, ConditionsJson, Recommendation, IsActive)
                VALUES ($code, $name, $conditions, $recommendation, $active)
                """;
        }
        else
        {
            command.CommandText = """
                UPDATE Rules
                SET Code = $code, Name = $name, ConditionsJson = $conditions, Recommendation = $recommendation, IsActive = $active
                WHERE Id = $id
                """;
            command.Parameters.AddWithValue("$id", record.Id);
        }

        command.Parameters.AddWithValue("$code", record.Code.Trim());
        command.Parameters.AddWithValue("$name", record.Name.Trim());
        command.Parameters.AddWithValue("$conditions", NormalizeConditions(record.ConditionsJson));
        command.Parameters.AddWithValue("$recommendation", record.Recommendation.ToString());
        command.Parameters.AddWithValue("$active", record.IsActive ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public void Toggle(int id)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Rules SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _factory.Create();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Rules WHERE Id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public static Rule ToDomainRule(RuleRecord record)
    {
        var conditions = JsonSerializer.Deserialize<List<RuleCondition>>(record.ConditionsJson) ?? new List<RuleCondition>();
        return new Rule(record.Code, record.Name, conditions, record.Recommendation, record.IsActive);
    }

    public static string NormalizeConditions(string conditionsJson)
    {
        var conditions = JsonSerializer.Deserialize<List<RuleCondition>>(conditionsJson) ?? new List<RuleCondition>();
        return JsonSerializer.Serialize(conditions);
    }
}
