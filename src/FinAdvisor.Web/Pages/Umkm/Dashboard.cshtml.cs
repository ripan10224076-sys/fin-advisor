using FinAdvisor.Web.Helpers;
using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Umkm;

public class DashboardModel : PageModel
{
    private readonly BusinessProfileRepository _profiles;
    private readonly ConsultationRepository _consultations;

    public DashboardModel(BusinessProfileRepository profiles, ConsultationRepository consultations)
    {
        _profiles = profiles;
        _consultations = consultations;
    }

    public BusinessProfile Profile { get; private set; } = new();
    public List<ConsultationRecord> RecentConsultations { get; private set; } = new();

    public void OnGet()
    {
        var userId = User.GetUserId();
        Profile = _profiles.Get(userId);
        RecentConsultations = _consultations.Search(null, null, userId).Take(5).ToList();
    }
}
