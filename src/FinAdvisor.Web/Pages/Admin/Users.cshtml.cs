using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

public class UsersModel : PageModel
{
    private readonly UserRepository _users;

    public UsersModel(UserRepository users)
    {
        _users = users;
    }

    public List<AppUser> Customers { get; private set; } = new();

    public void OnGet()
    {
        Customers = _users.GetAllCustomers();
    }
}
