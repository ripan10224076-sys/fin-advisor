using System.ComponentModel.DataAnnotations;

namespace FinAdvisor.Web.Models;

public class RegisterInput
{
    [Required(ErrorMessage = "Nama lengkap wajib diisi.")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Nama UMKM wajib diisi.")]
    public string BusinessName { get; set; } = "";

    [Required(ErrorMessage = "Kategori usaha wajib diisi.")]
    public string BusinessCategory { get; set; } = "";

    [Required(ErrorMessage = "Jenis usaha wajib diisi.")]
    public string BusinessType { get; set; } = "";

    [Required(ErrorMessage = "Alamat wajib diisi.")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Kota wajib diisi.")]
    public string City { get; set; } = "";

    [Required(ErrorMessage = "Provinsi wajib diisi.")]
    public string Province { get; set; } = "";

    [Required(ErrorMessage = "Nomor HP wajib diisi.")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Format email belum valid.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password wajib diisi.")]
    [MinLength(8, ErrorMessage = "Password minimal 8 karakter.")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Konfirmasi password wajib diisi.")]
    [Compare(nameof(Password), ErrorMessage = "Konfirmasi password tidak sama.")]
    public string ConfirmPassword { get; set; } = "";
}
