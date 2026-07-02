namespace FinAdvisor.Core.ExpertSystem;

public sealed record InferenceResult(Rule? FiredRule, IReadOnlyList<string> SatisfiedFacts, IReadOnlyList<Fact> WorkingFacts);
