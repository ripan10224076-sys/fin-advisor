using FinAdvisor.Web.Helpers;
using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Umkm;

public class ProfileModel : PageModel
{
    private readonly BusinessProfileRepository _profiles;
    private readonly UserRepository _users;

    public ProfileModel(BusinessProfileRepository profiles, UserRepository users)
    {
        _profiles = profiles;
        _users = users;
    }

    [BindProperty]
    public BusinessProfile Profile { get; set; } = new();

    public void OnGet()
    {
        Profile = _profiles.Get(User.GetUserId());
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = User.GetUserId();
        _profiles.Save(Profile, userId);
        if (userId.HasValue)
        {
            _users.UpdateCustomerProfile(userId.Value, Profile);
        }

        TempData["Toast"] = "Profil UMKM berhasil diperbarui.";
        return RedirectToPage();
    }
}
