using System.Security.Claims;
using FinAdvisor.Web.Models;
using FinAdvisor.Web.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FinAdvisor.Web.Services;

public class AuthService
{
    private readonly UserRepository _users;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(UserRepository users, PasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public AppUser? ValidateLogin(string email, string password, string requiredRole)
    {
        var user = _users.GetByEmail(email);
        if (user is null || !user.IsActive || user.Role != requiredRole)
        {
            return null;
        }

        return _passwordHasher.Verify(password, user.PasswordHash) ? user : null;
    }

    public async Task SignInAsync(HttpContext httpContext, AppUser user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("BusinessName", user.BusinessName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
    }

    public Task SignOutAsync(HttpContext httpContext) =>
        httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
