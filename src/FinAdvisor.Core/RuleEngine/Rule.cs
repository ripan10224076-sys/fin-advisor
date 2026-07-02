using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.ExpertSystem;

public sealed class Rule
{
    public Rule(string code, string name, IEnumerable<RuleCondition> conditions, RekomendasiJenis recommendation, bool isActive = true)
    {
        Code = code;
        Name = name;
        Conditions = conditions.ToList();
        Recommendation = recommendation;
        IsActive = isActive;
    }

    public string Code { get; }

    public string Name { get; }

    public IReadOnlyList<RuleCondition> Conditions { get; }

    public RekomendasiJenis Recommendation { get; }

    public bool IsActive { get; }

    public bool IsMatch(WorkingMemory memory) => IsActive && Conditions.All(condition => condition.IsSatisfiedBy(memory));

    public IReadOnlyList<string> GetSatisfiedFacts(WorkingMemory memory) =>
        Conditions.Where(condition => condition.IsSatisfiedBy(memory))
            .Select(condition => condition.ToHumanText())
            .ToList();
}
