using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Pages;

public class IndexModel : PageModel
{
    private readonly TenantProvider _tenantProvider;

    public IndexModel(TenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

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

        if(name != null)
        {
            AvailableAppTenants = _tenantProvider.GetAvailableTenants();
            AppTenantName = _tenantProvider.GetTenant(name).Text;

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();

            foreach (var role in roleClaims)
            {
                RolesInTenant.Add(role.Value);
            }

            TenantId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
        }
    }
}