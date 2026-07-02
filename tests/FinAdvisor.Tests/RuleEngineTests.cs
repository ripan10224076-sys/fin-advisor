using FinAdvisor.Core;
using FinAdvisor.Core.Models;
using Xunit;

namespace FinAdvisor.Tests;

public class RuleEngineTests
{
    private readonly RuleEngine _engine = new();

    [Fact]
    public void Kasus1_KasStabil_AsetKecil_KebutuhanSedang_HarusR3_Koperasi()
    {
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Kecil,
            KebutuhanDana = 50_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R3", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.Koperasi, hasil.Jenis);
    }

    [Fact]
    public void Kasus2_KasStabil_AgunanMemadai_LegalitasLengkap_HarusR2_Bank()
    {
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Memadai,
            KebutuhanDana = 100_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R2", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.Bank, hasil.Jenis);
    }

    [Fact]
    public void Kasus3_KasTidakStabil_RiwayatBuruk_HarusR1_TundaPinjaman()
    {
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.TidakStabil,
            AsetAgunan = AsetAgunan.Kecil,
            KebutuhanDana = 20_000_000m,
            RiwayatKredit = RiwayatKredit.Buruk,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R1", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.TundaPinjaman, hasil.Jenis);
    }

    [Fact]
    public void Kasus4_KebutuhanKecil_TanpaAgunan_Mendesak_HarusR4_Fintech()
    {
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.TidakAda,
            KebutuhanDana = 5_000_000m,
            RiwayatKredit = RiwayatKredit.BelumAda,
            LegalitasLengkap = false,
            ButuhDanaCepat = true
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R4", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.Fintech, hasil.Jenis);
    }

    [Fact]
    public void Kasus5_LegalitasTidakLengkap_KebutuhanBesar_HarusR5_LengkapiLegalitas()
    {
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Kecil,
            KebutuhanDana = 75_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = false,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R5", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.LengkapiLegalitas, hasil.Jenis);
    }

    [Fact]
    public void Kasus_TidakAdaAturanCocok_HarusFallback_PerluKonsultasi()
    {
        // Kondisi yang sengaja dibuat tidak memenuhi premis R1-R5 manapun.
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.Stabil,
            AsetAgunan = AsetAgunan.Memadai,
            KebutuhanDana = 5_000_000m,
            RiwayatKredit = RiwayatKredit.Baik,
            LegalitasLengkap = false,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("-", hasil.KodeAturan);
        Assert.Equal(RekomendasiJenis.PerluKonsultasi, hasil.Jenis);
    }

    [Fact]
    public void R1_DievaluasiLebihDahulu_DaripadaAturanLain()
    {
        // Meski agunan memadai & legalitas lengkap (mirip syarat R2),
        // arus kas tidak stabil + riwayat buruk seharusnya tetap men-trigger R1
        // karena R1 dievaluasi lebih dulu (mitigasi risiko didahulukan).
        var profile = new UmkmProfile
        {
            ArusKas = ArusKas.TidakStabil,
            AsetAgunan = AsetAgunan.Memadai,
            KebutuhanDana = 100_000_000m,
            RiwayatKredit = RiwayatKredit.Buruk,
            LegalitasLengkap = true,
            ButuhDanaCepat = false
        };

        var hasil = _engine.Evaluasi(profile);

        Assert.Equal("R1", hasil.KodeAturan);
    }
}
