using FinAdvisor.Web.Models;
using FinAdvisor.Web.Security;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages;

[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserRepository _users;
    private readonly PasswordHasher _passwordHasher;
    private readonly BusinessProfileRepository _profiles;
    private readonly AuthService _authService;

    public RegisterModel(
        UserRepository users,
        PasswordHasher passwordHasher,
        BusinessProfileRepository profiles,
        AuthService authService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _profiles = profiles;
        _authService = authService;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (_users.EmailExists(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Email sudah terdaftar.");
            return Page();
        }

        var userId = _users.CreateCustomer(Input, _passwordHasher.Hash(Input.Password));
        _profiles.Save(new BusinessProfile
        {
            BusinessName = Input.BusinessName,
            OwnerName = Input.FullName,
            Phone = Input.Phone,
            BusinessType = Input.BusinessType,
            Address = $"{Input.Address}, {Input.City}, {Input.Province}",
            MonthlyRevenue = "Belum diisi",
            FinancingPurpose = "Belum diisi"
        }, userId);

        var user = _users.GetById(userId);
        if (user is not null)
        {
            await _authService.SignInAsync(HttpContext, user, rememberMe: true);
        }

        return RedirectToPage("/Umkm/Dashboard");
    }
}
