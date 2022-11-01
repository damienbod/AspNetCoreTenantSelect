using AspNetCoreSelectTenant.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Pages;

public class AdminModel : PageModel
{
    private readonly TenantProvider _tenantProvider;
    private readonly TenantProviderCache _tenantProviderCache;

    public AdminModel(TenantProvider tenantProvider, TenantProviderCache tenantProviderCache)
    {
        _tenantProvider = tenantProvider;
        _tenantProviderCache = tenantProviderCache;
    }

    [BindProperty]
    public Tenant? Tenant { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Tenant != null)
        {
            var availableAppTenants = await _tenantProvider.GetAvailableTenantsAsync();

            if(availableAppTenants.Any(a => a.Text.ToLower() == Tenant.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "Tenant with this name already exists");
                return Page();
            }

            if (availableAppTenants.Any(a => a.Value.ToLower() == Tenant.TenantId.ToLower()))
            {
                ModelState.AddModelError("TenantId", "TenantId with this ID already exists");
                return Page();
            }

            await _tenantProvider.AddTenantAsync(Tenant);
        }

        return RedirectToPage("./Admin");
    }

    public void OnGet()
    {
    }
}