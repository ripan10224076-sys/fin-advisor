using System.Globalization;

namespace FinAdvisor.Web.Helpers;

public static class DisplayHelpers
{
    public static string Rupiah(decimal value) =>
        string.Create(CultureInfo.GetCultureInfo("id-ID"), $"Rp{value:N0}");
}
