using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string TenantId { get; set; } = string.Empty;

    [BindProperty]
    public List<string> RolesInTenant { get; set; } = new List<string>();

    [BindProperty]
    public string AppTenantName { get; set; } = string.Empty;

    [BindProperty]
    public List<SelectListItem> AvailableAppTenants { get; set; } = new List<SelectListItem>();

    public void OnGet()
    {
        var name = User.Identity!.Name;
        var claims = User.Claims.ToList();

        if(name != null)
        {
            AvailableAppTenants = TenantProvider.GetAvailableTenants(name);
            AppTenantName = TenantProvider.GetTenant(name);
        }

        List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();

        foreach (var role in roleClaims)
        {
            RolesInTenant.Add(role.Value);
        }

        TenantId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
    }
}