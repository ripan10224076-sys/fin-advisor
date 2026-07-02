namespace FinAdvisor.Web.Models;

public class ConsultationRecord
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BusinessName { get; set; } = "";
    public string InputJson { get; set; } = "";
    public string Recommendation { get; set; } = "";
    public string RuleCode { get; set; } = "";
    public int Score { get; set; }
    public string Explanation { get; set; } = "";
}
