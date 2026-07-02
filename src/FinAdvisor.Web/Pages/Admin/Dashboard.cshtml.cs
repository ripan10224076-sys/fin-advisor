using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

public class DashboardModel : PageModel
{
    private readonly AdminDashboardService _dashboardService;

    public DashboardModel(AdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public AdminDashboardStats Stats { get; private set; } = new();

    public List<ConsultationRecord> RecentActivities { get; private set; } = new();

    public void OnGet()
    {
        Stats = _dashboardService.GetStats();
        RecentActivities = _dashboardService.GetRecentConsultations();
    }
}
