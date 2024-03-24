using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using AspNetCoreSelectTenant.Tenants;

namespace AspNetCoreSelectTenant.Pages;

public class SwitchTenantModel : PageModel
{
    private readonly TenantProvider _tenantProvider;
    private readonly TenantProviderCache _tenantProviderCache;

    public SwitchTenantModel(TenantProvider tenantProvider, TenantProviderCache tenantProviderCache)
    {
        _tenantProvider = tenantProvider;
        _tenantProviderCache = tenantProviderCache;
    }

    [BindProperty]
    public string Domain { get; set; } = string.Empty;

    [BindProperty]
    public string TenantId { get; set; } = string.Empty;

    [BindProperty]
    public List<string> RolesInTenant { get; set; } = new List<string>();

    [BindProperty]
    public string AppTenantName { get; set; } = string.Empty;

    [BindProperty]
    public List<SelectListItem> AvailableAppTenants { get; set; } = new List<SelectListItem>();

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

            var tid = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");

            if (tid != null) TenantId = tid;
        }
    }

    /// <summary>
    /// Only works from a direct GET, not a post or a redirect
    /// </summary>
    public async Task<IActionResult> OnGetSignIn([FromQuery]string domain)
    {
        var email = User.Identity!.Name;
        if(email != null)
        {
            var organization = await _tenantProvider.GetTenantForOrg(domain);
            _tenantProviderCache.SetTenant(email, organization);
        }

        return Challenge(new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme);
    }
}