using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

public class StatisticsModel : PageModel
{
    private readonly AdminDashboardService _dashboardService;

    public StatisticsModel(AdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public AdminDashboardStats Stats { get; private set; } = new();
    public List<ConsultationRecord> Recent { get; private set; } = new();

    public void OnGet()
    {
        Stats = _dashboardService.GetStats();
        Recent = _dashboardService.GetRecentConsultations(12);
    }
}
