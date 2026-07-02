using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages;

public class HistoryModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Umkm/History");
}
