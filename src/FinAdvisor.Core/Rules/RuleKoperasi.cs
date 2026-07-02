using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// R3: IF arus kas stabil AND aset kecil AND kebutuhan dana sedang
///     THEN rekomendasi = Koperasi.
/// </summary>
public class RuleKoperasi : IRule
{
    public string Kode => "R3";

    public string Deskripsi =>
        "Arus kas stabil DAN aset/agunan kecil DAN kebutuhan dana kategori sedang";

    public bool IsMatch(UmkmProfile profile) =>
        profile.ArusKas == ArusKas.Stabil &&
        profile.AsetAgunan == AsetAgunan.Kecil &&
        KebutuhanDanaKategori.IsSedang(profile.KebutuhanDana);

    public HasilRekomendasi GetResult(UmkmProfile profile) => new(
        RekomendasiJenis.Koperasi,
        Kode,
        "Arus kas stabil namun agunan yang dimiliki terbatas, dengan kebutuhan dana " +
        "pada kisaran menengah. Koperasi umumnya memberikan syarat yang lebih fleksibel " +
        "dibanding bank untuk profil usaha seperti ini.");
}
