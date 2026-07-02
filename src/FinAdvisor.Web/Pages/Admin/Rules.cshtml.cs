using System.Text.Json;
using FinAdvisor.Core.Models;
using FinAdvisor.Core.ExpertSystem;
using FinAdvisor.Web.Models;
using FinAdvisor.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinAdvisor.Web.Pages.Admin;

public class RulesModel : PageModel
{
    private readonly RuleRepository _repository;

    public RulesModel(RuleRepository repository)
    {
        _repository = repository;
    }

    [BindProperty]
    public RuleRecord Form { get; set; } = new();

    public List<RuleRecord> Rules { get; private set; } = new();

    public IEnumerable<string> RecommendationOptions => Enum.GetNames<RekomendasiJenis>();

    public void OnGet(int? editId)
    {
        Load(editId);
    }

    public IActionResult OnPostSave()
    {
        if (string.IsNullOrWhiteSpace(Form.Code) || string.IsNullOrWhiteSpace(Form.Name) || string.IsNullOrWhiteSpace(Form.ConditionsJson))
        {
            TempData["Toast"] = "Kode, nama, dan kondisi rule wajib diisi.";
            Load(Form.Id == 0 ? null : Form.Id);
            return Page();
        }

        try
        {
            _repository.Save(Form);
            TempData["Toast"] = "Rule berhasil disimpan.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            TempData["Toast"] = $"Rule belum valid: {ex.Message}";
            Load(Form.Id == 0 ? null : Form.Id);
            return Page();
        }
    }

    public IActionResult OnPostToggle(int id)
    {
        _repository.Toggle(id);
        TempData["Toast"] = "Status rule diperbarui.";
        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        _repository.Delete(id);
        TempData["Toast"] = "Rule dihapus.";
        return RedirectToPage();
    }

    private void Load(int? editId)
    {
        Rules = _repository.GetAll();
        Form = editId.HasValue
            ? _repository.GetById(editId.Value) ?? NewRule()
            : NewRule();
    }

    private static RuleRecord NewRule() => new()
    {
        Code = "R6",
        Name = "Rule Baru",
        ConditionsJson = JsonSerializer.Serialize(new[] { new RuleCondition("ArusKas", "Stabil") }, new JsonSerializerOptions { WriteIndented = true }),
        Recommendation = RekomendasiJenis.Koperasi,
        IsActive = true
    };
}
