using FinAdvisor.Web.Models;
using FinAdvisor.Web.Security;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AuthService _authService;

    public LoginModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _authService.ValidateLogin(Input.Email, Input.Password, AppRoles.Administrator);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Email atau password admin belum sesuai.");
            return Page();
        }

        await _authService.SignInAsync(HttpContext, user, Input.RememberMe);
        return RedirectToPage("/Admin/Dashboard");
    }
}
