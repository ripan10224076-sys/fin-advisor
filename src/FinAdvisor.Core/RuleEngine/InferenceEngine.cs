namespace FinAdvisor.Core.ExpertSystem;

public class InferenceEngine
{
    public InferenceResult Run(WorkingMemory memory, IEnumerable<Rule> rules)
    {
        foreach (var rule in rules)
        {
            if (rule.IsMatch(memory))
            {
                memory.Add("Rekomendasi", rule.Recommendation.ToString());
                return new InferenceResult(rule, rule.GetSatisfiedFacts(memory), memory.Facts);
            }
        }

        return new InferenceResult(null, Array.Empty<string>(), memory.Facts);
    }
}
