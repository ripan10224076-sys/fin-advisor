using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Rules;

/// <summary>
/// R5: IF legalitas tidak lengkap AND kebutuhan dana besar
///     THEN rekomendasi = Lengkapi Legalitas
///          (sementara arahkan ke Koperasi/Fintech).
/// </summary>
public class RuleLengkapiLegalitas : IRule
{
    public string Kode => "R5";

    public string Deskripsi =>
        "Legalitas usaha belum lengkap DAN kebutuhan dana kategori besar";

    public bool IsMatch(UmkmProfile profile) =>
        !profile.LegalitasLengkap &&
        KebutuhanDanaKategori.IsBesar(profile.KebutuhanDana);

    public HasilRekomendasi GetResult(UmkmProfile profile) => new(
        RekomendasiJenis.LengkapiLegalitas,
        Kode,
        "Kebutuhan dana tergolong besar, namun legalitas usaha (NIB/NPWP) belum lengkap " +
        "sehingga sulit memenuhi syarat bank. Sebaiknya lengkapi legalitas usaha terlebih " +
        "dahulu; untuk sementara, koperasi atau fintech dapat menjadi opsi jangka pendek.");
}
