namespace FinAdvisor.Core.Models;

/// <summary>
/// Kategorisasi nominal kebutuhan dana menjadi Kecil / Sedang / Besar.
/// Ambang batas (threshold) sengaja dibuat sebagai konstanta agar mudah
/// disesuaikan pada saat validasi dengan narasumber/pakar (lihat Bab 3.4
/// catatan pada Dokumen Perancangan).
/// </summary>
public static class KebutuhanDanaKategori
{
    public const decimal BatasKecil = 10_000_000m;   // < 10 juta => Kecil
    public const decimal BatasSedang = 50_000_000m;  // 10-50 juta => Sedang, > 50 juta => Besar

    public static bool IsKecil(decimal nominal) => nominal < BatasKecil;

    public static bool IsSedang(decimal nominal) => nominal >= BatasKecil && nominal <= BatasSedang;

    public static bool IsBesar(decimal nominal) => nominal > BatasSedang;
}
