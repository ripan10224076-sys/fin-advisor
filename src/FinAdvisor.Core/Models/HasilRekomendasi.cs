namespace FinAdvisor.Core.Models;

/// <summary>
/// Hasil akhir yang dikembalikan mesin inferensi: jenis rekomendasi,
/// kode aturan yang terpicu, dan penjelasan dalam bahasa non-teknis
/// (fitur "explanation facility" pada Bab 3.6 Dokumen Perancangan).
/// </summary>
public class HasilRekomendasi
{
    public HasilRekomendasi(RekomendasiJenis jenis, string kodeAturan, string penjelasan)
    {
        Jenis = jenis;
        KodeAturan = kodeAturan;
        Penjelasan = penjelasan;
    }

    public RekomendasiJenis Jenis { get; }

    /// <summary>Kode aturan yang terpicu, misalnya "R2". "-" jika tidak ada aturan yang cocok.</summary>
    public string KodeAturan { get; }

    public string Penjelasan { get; }

    public int SkorKesesuaian { get; init; } = 60;

    public IReadOnlyList<string> FaktaTerpenuhi { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Keuntungan { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Risiko { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Persyaratan { get; init; } = Array.Empty<string>();

    public string EstimasiProses { get; init; } = "Perlu verifikasi lanjutan";

    public IReadOnlyList<string> ChecklistDokumen { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> SaranTambahan { get; init; } = Array.Empty<string>();

    public override string ToString() => $"[{KodeAturan}] {Jenis}: {Penjelasan}";
}
