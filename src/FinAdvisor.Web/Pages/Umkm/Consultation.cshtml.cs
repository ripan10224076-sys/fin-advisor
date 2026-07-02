using FinAdvisor.Core;
using FinAdvisor.Core.KnowledgeBase;
using FinAdvisor.Core.Models;
using FinAdvisor.Core.Services;
using FinAdvisor.Web.Helpers;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Umkm;

public class ConsultationModel : PageModel
{
    private readonly RuleRepository _ruleRepository;
    private readonly ConsultationRepository _consultationRepository;
    private readonly BusinessProfileRepository _businessProfileRepository;

    public ConsultationModel(
        RuleRepository ruleRepository,
        ConsultationRepository consultationRepository,
        BusinessProfileRepository businessProfileRepository)
    {
        _ruleRepository = ruleRepository;
        _consultationRepository = consultationRepository;
        _businessProfileRepository = businessProfileRepository;
    }

    [BindProperty]
    public string BusinessName { get; set; } = "";

    [BindProperty]
    public UmkmProfile Profile { get; set; } = new();

    public HasilRekomendasi? Hasil { get; private set; }
    public string OwnerName { get; private set; } = "";

    public void OnGet()
    {
        LoadBusinessProfile();
    }

    public IActionResult OnPost()
    {
        LoadBusinessProfile();
        if (!ModelState.IsValid)
        {
            TempData["Toast"] = "Lengkapi data konsultasi dengan benar.";
            return Page();
        }

        var rules = _ruleRepository.GetActiveDomainRules();
        var engine = new RuleEngine(new RecommendationService(new KnowledgeBase(rules)));
        Hasil = engine.Evaluasi(Profile);
        _consultationRepository.Add(BusinessName, Profile, Hasil, User.GetUserId());
        TempData["Toast"] = "Analisis berhasil disimpan ke riwayat.";
        return Page();
    }

    private void LoadBusinessProfile()
    {
        var businessProfile = _businessProfileRepository.Get(User.GetUserId());
        if (string.IsNullOrWhiteSpace(BusinessName))
        {
            BusinessName = businessProfile.BusinessName;
        }

        OwnerName = businessProfile.OwnerName;
    }
}
