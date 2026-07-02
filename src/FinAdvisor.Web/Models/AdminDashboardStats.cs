namespace FinAdvisor.Web.Models;

public class AdminDashboardStats
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalConsultations { get; set; }
    public int TotalRules { get; set; }
    public int ActiveRules { get; set; }
    public int TodayConsultations { get; set; }
}
