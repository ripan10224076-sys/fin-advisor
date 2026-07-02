using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages;

public class LogoutModel : PageModel
{
    private readonly AuthService _authService;

    public LogoutModel(AuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _authService.SignOutAsync(HttpContext);
        return RedirectToPage("/Index");
    }
}
