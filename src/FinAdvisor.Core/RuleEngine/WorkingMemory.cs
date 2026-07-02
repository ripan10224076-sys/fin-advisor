using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.ExpertSystem;

public sealed class WorkingMemory
{
    private readonly List<Fact> _facts = new();

    public WorkingMemory(UmkmProfile profile)
    {
        Profile = profile;
        Add("ArusKas", profile.ArusKas.ToString());
        Add("Agunan", profile.AsetAgunan.ToString());
        Add("Dana", GetDanaKategori(profile.KebutuhanDana));
        Add("RiwayatKredit", profile.RiwayatKredit.ToString());
        Add("Legalitas", profile.LegalitasLengkap ? "Lengkap" : "TidakLengkap");
        Add("ButuhDanaCepat", profile.ButuhDanaCepat ? "Ya" : "Tidak");
    }

    public UmkmProfile Profile { get; }

    public IReadOnlyList<Fact> Facts => _facts;

    public void Add(string name, string value) => _facts.Add(new Fact(name, value));

    public bool Has(string name, string value) =>
        _facts.Any(f => string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(f.Value, value, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<string> DescribeFacts() => _facts.Select(f => $"{ToLabel(f.Name)}: {ToDisplayValue(f.Value)}").ToList();

    private static string GetDanaKategori(decimal nominal)
    {
        if (KebutuhanDanaKategori.IsKecil(nominal))
        {
            return "Kecil";
        }

        return KebutuhanDanaKategori.IsSedang(nominal) ? "Sedang" : "Besar";
    }

    private static string ToLabel(string name) => name switch
    {
        "ArusKas" => "Arus kas",
        "Agunan" => "Agunan",
        "Dana" => "Kategori dana",
        "RiwayatKredit" => "Riwayat kredit",
        "Legalitas" => "Legalitas",
        "ButuhDanaCepat" => "Dana mendesak",
        _ => name
    };

    private static string ToDisplayValue(string value) => value switch
    {
        "TidakStabil" => "Tidak stabil",
        "TidakAda" => "Tidak ada",
        "TidakLengkap" => "Tidak lengkap",
        "BelumAda" => "Belum ada",
        "Ya" => "Ya",
        "Tidak" => "Tidak",
        _ => value
    };
}
