namespace FinAdvisor.Core.ExpertSystem;

public sealed record RuleCondition(string FactName, string ExpectedValue)
{
    public bool IsSatisfiedBy(WorkingMemory memory) => memory.Has(FactName, ExpectedValue);

    public string ToHumanText() => $"{FactName} = {ExpectedValue}";
}
