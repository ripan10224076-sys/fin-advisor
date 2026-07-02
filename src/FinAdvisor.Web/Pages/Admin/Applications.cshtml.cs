using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

public class ApplicationsModel : PageModel
{
    private readonly AdminDashboardService _dashboardService;

    public ApplicationsModel(AdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public List<ConsultationRecord> Items { get; private set; } = new();

    public void OnGet()
    {
        Items = _dashboardService.GetRecentConsultations(25);
    }
}
