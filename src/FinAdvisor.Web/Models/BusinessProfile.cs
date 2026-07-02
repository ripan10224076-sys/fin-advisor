using System.ComponentModel.DataAnnotations;

namespace FinAdvisor.Web.Models;

public class BusinessProfile
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nama usaha wajib diisi.")]
    public string BusinessName { get; set; } = "";

    [Required(ErrorMessage = "Nama pemilik wajib diisi.")]
    public string OwnerName { get; set; } = "";

    [Required(ErrorMessage = "Nomor kontak wajib diisi.")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Jenis usaha wajib diisi.")]
    public string BusinessType { get; set; } = "";

    [Required(ErrorMessage = "Alamat usaha wajib diisi.")]
    public string Address { get; set; } = "";

    public string MonthlyRevenue { get; set; } = "";

    public string FinancingPurpose { get; set; } = "";

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
