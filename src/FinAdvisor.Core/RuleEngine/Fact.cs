namespace FinAdvisor.Core.ExpertSystem;

public sealed record Fact(string Name, string Value)
{
    public string Key => $"{Name}={Value}";
}
