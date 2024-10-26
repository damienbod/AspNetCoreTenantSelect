using AspNetCoreSelectTenant.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Pages;

public class IndexModel : PageModel
{
    private readonly TenantProvider _tenantProvider;
    private readonly TenantProviderCache _tenantProviderCache;

    public IndexModel(TenantProvider tenantProvider, TenantProviderCache tenantProviderCache)
    {
        _tenantProvider = tenantProvider;
        _tenantProviderCache = tenantProviderCache;
    }

    [BindProperty]
    public string TenantId { get; set; } = string.Empty;

    [BindProperty]
    public List<string> RolesInTenant { get; set; } = [];

    [BindProperty]
    public string AppTenantName { get; set; } = string.Empty;

    [BindProperty]
    public List<SelectListItem> AvailableAppTenants { get; set; } = [];

    public async Task OnGetAsync()
    {
        var name = User.Identity!.Name;

        if (name != null)
        {
            AvailableAppTenants = await _tenantProvider.GetAvailableTenantsAsync();
            AppTenantName = _tenantProviderCache.GetTenant(name).Text;

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();

            foreach (var role in roleClaims)
            {
                RolesInTenant.Add(role.Value);
            }

            TenantId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
        }
    }
}