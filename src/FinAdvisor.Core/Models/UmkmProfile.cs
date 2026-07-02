using System.ComponentModel.DataAnnotations;

namespace FinAdvisor.Core.Models;

/// <summary>
/// Kumpulan fakta (input) mengenai kondisi UMKM yang dimasukkan pengguna
/// melalui antarmuka. Objek ini berperan sebagai "working memory" pada
/// arsitektur sistem pakar (lihat Dokumen Perancangan Bab 3.6).
/// </summary>
public class UmkmProfile
{
    [Required]
    public ArusKas ArusKas { get; set; }

    [Required]
    public AsetAgunan AsetAgunan { get; set; }

    /// <summary>Nominal kebutuhan dana dalam Rupiah.</summary>
    [Range(1, 10_000_000_000, ErrorMessage = "Nominal dana harus lebih dari Rp0.")]
    public decimal KebutuhanDana { get; set; }

    [Required]
    public RiwayatKredit RiwayatKredit { get; set; }

    /// <summary>True jika legalitas usaha (NIB/NPWP) sudah lengkap.</summary>
    public bool LegalitasLengkap { get; set; }

    /// <summary>True jika pengguna menyatakan butuh dana secara cepat/mendesak.</summary>
    public bool ButuhDanaCepat { get; set; }
}
