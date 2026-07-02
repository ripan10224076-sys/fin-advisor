using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// R2: IF arus kas stabil AND aset/agunan memadai AND legalitas lengkap
///     THEN rekomendasi = Bank.
/// </summary>
public class RuleBank : IRule
{
    public string Kode => "R2";

    public string Deskripsi =>
        "Arus kas stabil DAN aset/agunan memadai DAN legalitas usaha lengkap";

    public bool IsMatch(UmkmProfile profile) =>
        profile.ArusKas == ArusKas.Stabil &&
        profile.AsetAgunan == AsetAgunan.Memadai &&
        profile.LegalitasLengkap;

    public HasilRekomendasi GetResult(UmkmProfile profile) => new(
        RekomendasiJenis.Bank,
        Kode,
        "Arus kas usaha stabil, agunan memadai, dan legalitas usaha sudah lengkap. " +
        "Usaha Anda memenuhi syarat umum kredit bank yang menawarkan bunga relatif " +
        "lebih rendah dibanding koperasi maupun fintech.");
}
