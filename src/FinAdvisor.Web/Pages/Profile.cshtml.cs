using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages;

public class ProfileModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/Umkm/Profile");
}
