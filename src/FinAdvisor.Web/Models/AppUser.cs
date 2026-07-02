namespace FinAdvisor.Web.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string BusinessCategory { get; set; } = "";
    public string BusinessType { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string Province { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
