using FinAdvisor.Core.Models;
using FinAdvisor.Core.ExpertSystem;
using KnowledgeBaseModel = FinAdvisor.Core.KnowledgeBase.KnowledgeBase;

namespace FinAdvisor.Core.Services;

public class RecommendationService
{
    private readonly InferenceEngine _inferenceEngine;
    private readonly KnowledgeBaseModel _knowledgeBase;

    public RecommendationService()
        : this(new KnowledgeBaseModel())
    {
    }

    public RecommendationService(KnowledgeBaseModel knowledgeBase)
    {
        _knowledgeBase = knowledgeBase;
        _inferenceEngine = new InferenceEngine();
    }

    public HasilRekomendasi Analyze(UmkmProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var memory = new WorkingMemory(profile);
        var result = _inferenceEngine.Run(memory, _knowledgeBase.Rules);

        if (result.FiredRule is null)
        {
            return BuildFallback(memory);
        }

        return BuildRecommendation(result.FiredRule, result.SatisfiedFacts);
    }

    private static HasilRekomendasi BuildRecommendation(Rule rule, IReadOnlyList<string> satisfiedFacts)
    {
        var details = RecommendationCatalog.Get(rule.Recommendation);
        return new HasilRekomendasi(rule.Recommendation, rule.Code, details.Explanation)
        {
            SkorKesesuaian = details.Score,
            FaktaTerpenuhi = satisfiedFacts,
            Keuntungan = details.Benefits,
            Risiko = details.Risks,
            Persyaratan = details.Requirements,
            EstimasiProses = details.EstimatedProcess,
            ChecklistDokumen = details.DocumentChecklist,
            SaranTambahan = details.Suggestions
        };
    }

    private static HasilRekomendasi BuildFallback(WorkingMemory memory) => new(
        RekomendasiJenis.PerluKonsultasi,
        "-",
        "Data usaha belum cocok dengan aturan utama FIN-ADVISOR. Sistem menyarankan konsultasi lanjutan agar pilihan pembiayaan tidak dipaksakan.")
    {
        SkorKesesuaian = 45,
        FaktaTerpenuhi = memory.DescribeFacts(),
        Keuntungan = new[] { "Keputusan tidak dipaksakan saat data belum kuat", "Memberi ruang evaluasi oleh pendamping UMKM" },
        Risiko = new[] { "Proses membutuhkan waktu tambahan", "Rekomendasi belum spesifik ke satu lembaga" },
        Persyaratan = new[] { "Lengkapi data usaha", "Siapkan catatan penjualan dan pengeluaran" },
        EstimasiProses = "1-3 hari untuk konsultasi awal",
        ChecklistDokumen = new[] { "KTP pemilik", "Catatan omzet", "Data legalitas yang tersedia" },
        SaranTambahan = new[] { "Periksa kembali nominal dana dan kesiapan dokumen", "Konsultasikan ke pendamping UMKM atau lembaga keuangan terdekat" }
    };
}
