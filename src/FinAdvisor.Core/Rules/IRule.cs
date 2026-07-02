using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// Kontrak untuk satu aturan produksi (production rule) berbentuk IF-THEN
/// sebagaimana didefinisikan pada Dokumen Perancangan Bab 3.4.
/// </summary>
public interface IRule
{
    /// <summary>Kode aturan, contoh: "R1", "R2", dst.</summary>
    string Kode { get; }

    /// <summary>Deskripsi singkat aturan (bagian IF).</summary>
    string Deskripsi { get; }

    /// <summary>Mengecek apakah kondisi (premis) aturan ini terpenuhi oleh fakta yang ada.</summary>
    bool IsMatch(UmkmProfile profile);

    /// <summary>Menghasilkan kesimpulan (bagian THEN) berikut penjelasannya.</summary>
    HasilRekomendasi GetResult(UmkmProfile profile);
}
