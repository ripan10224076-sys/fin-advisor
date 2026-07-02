using FinAdvisor.Core.Models;
using FinAdvisor.Core.ExpertSystem;

namespace FinAdvisor.Core.KnowledgeBase;

public class KnowledgeBase
{
    public KnowledgeBase()
        : this(GetDefaultRules())
    {
    }

    public KnowledgeBase(IEnumerable<Rule> rules)
    {
        Rules = rules.ToList();
    }

    public IReadOnlyList<Rule> Rules { get; }

    public static IReadOnlyList<Rule> GetDefaultRules() => new List<Rule>
    {
        new("R1", "Tunda Pinjaman", new[]
        {
            new RuleCondition("ArusKas", "TidakStabil"),
            new RuleCondition("RiwayatKredit", "Buruk")
        }, RekomendasiJenis.TundaPinjaman),
        new("R2", "Bank", new[]
        {
            new RuleCondition("ArusKas", "Stabil"),
            new RuleCondition("Agunan", "Memadai"),
            new RuleCondition("Legalitas", "Lengkap")
        }, RekomendasiJenis.Bank),
        new("R3", "Koperasi", new[]
        {
            new RuleCondition("ArusKas", "Stabil"),
            new RuleCondition("Agunan", "Kecil"),
            new RuleCondition("Dana", "Sedang")
        }, RekomendasiJenis.Koperasi),
        new("R4", "Fintech", new[]
        {
            new RuleCondition("Dana", "Kecil"),
            new RuleCondition("Agunan", "TidakAda"),
            new RuleCondition("ButuhDanaCepat", "Ya")
        }, RekomendasiJenis.Fintech),
        new("R5", "Lengkapi Legalitas", new[]
        {
            new RuleCondition("Legalitas", "TidakLengkap"),
            new RuleCondition("Dana", "Besar")
        }, RekomendasiJenis.LengkapiLegalitas)
    };
}
