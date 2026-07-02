using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// R4: IF kebutuhan dana kecil AND tanpa agunan AND butuh dana cepat
///     THEN rekomendasi = Fintech.
/// </summary>
public class RuleFintech : IRule
{
    public string Kode => "R4";

    public string Deskripsi =>
        "Kebutuhan dana kecil DAN tanpa agunan DAN butuh dana cepat";

    public bool IsMatch(UmkmProfile profile) =>
        KebutuhanDanaKategori.IsKecil(profile.KebutuhanDana) &&
        profile.AsetAgunan == AsetAgunan.TidakAda &&
        profile.ButuhDanaCepat;

    public HasilRekomendasi GetResult(UmkmProfile profile) => new(
        RekomendasiJenis.Fintech,
        Kode,
        "Kebutuhan dana relatif kecil, tidak memiliki agunan, dan pencairan dibutuhkan " +
        "secara cepat. Fintech lending (yang terdaftar/diawasi OJK) dapat menjadi pilihan, " +
        "namun perlu diperhatikan bunga yang cenderung lebih tinggi dibanding bank/koperasi.");
}
