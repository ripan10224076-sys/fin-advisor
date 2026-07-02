using FinAdvisor.Core;
using FinAdvisor.Core.Models;

var engine = new RuleEngine();

Console.WriteLine("=========================================");
Console.WriteLine(" FIN-ADVISOR - Demo Rekomendasi Pembiayaan");
Console.WriteLine("=========================================\n");

// Menjalankan 4 skenario contoh sesuai Dokumen Perancangan Bab 3.7 (Contoh Kasus / Test Case)
var skenario = new (string Judul, UmkmProfile Profile)[]
{
    ("Kasus 1: Kas stabil, aset kecil, butuh Rp50 juta, legalitas lengkap (harapan: R3 - Koperasi)",
        new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Kecil,
            KebutuhanDana = 50_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        }),

    ("Kasus 2: Kas stabil, agunan memadai, legalitas lengkap, butuh Rp100 juta (harapan: R2 - Bank)",
        new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Memadai,
            KebutuhanDana = 100_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        }),

    ("Kasus 3: Kas tidak stabil, riwayat kredit buruk (harapan: R1 - Tunda Pinjaman)",
        new UmkmProfile
        {
            ArusKas = ArusKas.TidakStabil,
            AsetAgunan = AsetAgunan.Kecil,
            KebutuhanDana = 20_000_000m,
            RiwayatKredit = RiwayatKredit.Buruk,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        }),

    ("Kasus 4: Butuh Rp5 juta, tanpa agunan, mendesak (harapan: R4 - Fintech)",
        new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.TidakAda,
            KebutuhanDana = 5_000_000m,
            RiwayatKredit = RiwayatKredit.BelumAda,
            LegalitasLengkap = false,
            ButuhDanaCepat = true
        }),
};

foreach (var (judul, profile) in skenario)
{
    var hasil = engine.Evaluasi(profile);
    Console.WriteLine(judul);
    Console.WriteLine($"  -> Aturan terpicu : {hasil.KodeAturan}");
    Console.WriteLine($"  -> Rekomendasi     : {hasil.Jenis}");
    Console.WriteLine($"  -> Penjelasan      : {hasil.Penjelasan}");
    Console.WriteLine();
}

Console.WriteLine("-----------------------------------------");
Console.WriteLine("Coba masukkan data Anda sendiri? (y/n)");
if (Console.ReadLine()?.Trim().ToLowerInvariant() == "y")
{
    var profile = BacaInputDariPengguna();
    var hasil = engine.Evaluasi(profile);

    Console.WriteLine("\n=== HASIL REKOMENDASI ===");
    Console.WriteLine($"Aturan terpicu : {hasil.KodeAturan}");
    Console.WriteLine($"Rekomendasi    : {hasil.Jenis}");
    Console.WriteLine($"Penjelasan     : {hasil.Penjelasan}");
}

static UmkmProfile BacaInputDariPengguna()
{
    Console.Write("Arus kas stabil? (y/n): ");
    var arusKas = (Console.ReadLine()?.Trim().ToLowerInvariant() == "y") ? ArusKas.Stabil : ArusKas.TidakStabil;

    Console.Write("Aset/agunan (1=Memadai, 2=Kecil, 3=TidakAda): ");
    var asetInput = Console.ReadLine()?.Trim();
    var aset = asetInput switch
    {
        "1" => AsetAgunan.Memadai,
        "2" => AsetAgunan.Kecil,
        _ => AsetAgunan.TidakAda
    };

    Console.Write("Kebutuhan dana (Rp): ");
    decimal.TryParse(Console.ReadLine(), out var kebutuhan);

    Console.Write("Riwayat kredit (1=Baik, 2=Buruk, 3=BelumAda): ");
    var riwayatInput = Console.ReadLine()?.Trim();
    var riwayat = riwayatInput switch
    {
        "1" => RiwayatKredit.Baik,
        "2" => RiwayatKredit.Buruk,
        _ => RiwayatKredit.BelumAda
    };

    Console.Write("Legalitas usaha lengkap? (y/n): ");
    var legalitas = Console.ReadLine()?.Trim().ToLowerInvariant() == "y";

    Console.Write("Butuh dana cepat/mendesak? (y/n): ");
    var cepat = Console.ReadLine()?.Trim().ToLowerInvariant() == "y";

    return new UmkmProfile
    {
        ArusKas = arusKas,
        AsetAgunan = aset,
        KebutuhanDana = kebutuhan,
        RiwayatKredit = riwayat,
        LegalitasLengkap = legalitas,
        ButuhDanaCepat = cepat
    };
}
