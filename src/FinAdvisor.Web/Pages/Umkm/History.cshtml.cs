using FinAdvisor.Core.Models;
using FinAdvisor.Web.Helpers;
using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Umkm;

public class HistoryModel : PageModel
{
    private readonly ConsultationRepository _repository;

    public HistoryModel(ConsultationRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Recommendation { get; set; }

    public List<ConsultationRecord> Items { get; private set; } = new();

    public IEnumerable<string> RecommendationOptions => Enum.GetNames<RekomendasiJenis>();

    public void OnGet()
    {
        Items = _repository.Search(Query, Recommendation, User.GetUserId());
    }
}
