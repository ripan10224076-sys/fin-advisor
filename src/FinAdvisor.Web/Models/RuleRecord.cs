using FinAdvisor.Core.Models;

namespace FinAdvisor.Web.Models;

public class RuleRecord
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string ConditionsJson { get; set; } = "[]";
    public RekomendasiJenis Recommendation { get; set; }
    public bool IsActive { get; set; } = true;
}
