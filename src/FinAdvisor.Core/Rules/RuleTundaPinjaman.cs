using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// R1: IF arus kas tidak stabil AND riwayat kredit buruk
///     THEN rekomendasi = Tunda Pinjaman.
/// </summary>
public class RuleTundaPinjaman : IRule
{
    public string Kode => "R1";

    public string Deskripsi =>
        "Arus kas tidak stabil DAN riwayat kredit buruk";

    public bool IsMatch(UmkmProfile profile) =>
        profile.ArusKas == ArusKas.TidakStabil &&
        profile.RiwayatKredit == RiwayatKredit.Buruk;

    public HasilRekomendasi GetResult(UmkmProfile profile) => new(
        RekomendasiJenis.TundaPinjaman,
        Kode,
        "Kondisi arus kas belum stabil dan riwayat kredit tercatat kurang baik. " +
        "Sebaiknya perbaiki dahulu pengelolaan arus kas usaha sebelum mengajukan " +
        "pinjaman baru, agar tidak menambah beban keuangan.");
}
