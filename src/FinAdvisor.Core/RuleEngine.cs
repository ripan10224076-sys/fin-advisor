using FinAdvisor.Core.Models;
using FinAdvisor.Core.Services;

namespace FinAdvisor.Core;

/// <summary>
/// Mesin inferensi FIN-ADVISOR menggunakan metode forward chaining:
/// aturan dievaluasi berurutan (R1 -> R5) terhadap fakta (UmkmProfile)
/// yang diberikan, dan aturan PERTAMA yang cocok menentukan rekomendasi
/// akhir. Urutan ini mengikuti diagram alur inferensi pada Dokumen
/// Perancangan Bab 3.5.
/// </summary>
public class RuleEngine
{
    private readonly RecommendationService _recommendationService;

    /// <summary>
    /// Konstruktor default: memuat kelima aturan (R1-R5) sesuai basis
    /// pengetahuan pada Dokumen Perancangan Bab 3.4, dalam urutan evaluasi
    /// yang sudah ditentukan.
    /// </summary>
    public RuleEngine()
    {
        _recommendationService = new RecommendationService();
    }

    public RuleEngine(RecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    /// <summary>
    /// Menjalankan proses forward chaining: menelusuri aturan satu per satu
    /// dan mengembalikan hasil dari aturan pertama yang premisnya terpenuhi.
    /// Jika tidak ada aturan yang cocok, sistem mengembalikan rekomendasi
    /// fallback berupa anjuran konsultasi lanjutan.
    /// </summary>
    public HasilRekomendasi Evaluasi(UmkmProfile profile)
    {
        return _recommendationService.Analyze(profile);
    }
}
