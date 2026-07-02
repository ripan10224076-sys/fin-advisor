using System.ComponentModel.DataAnnotations;

namespace FinAdvisor.Web.Models;

public class LoginInput
{
    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email belum valid.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password wajib diisi.")]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }
}
