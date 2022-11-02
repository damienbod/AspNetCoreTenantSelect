using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSelectTenant.Tenants;

/// <summary>
/// Note, no restrictions are in added. You need to authorize the iss and/or the tid claim after an authentication
/// when only you tenants are allowed to access. 
/// </summary>
public class TenantProvider
{
    private static readonly SelectListItem _common = new("common", "common");

    private readonly TenantContext _tenantContext;

    public TenantProvider(TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task<List<SelectListItem>> GetAvailableTenantsAsync()
    {
        var items = await _tenantContext.Tenants.ToListAsync();
        return items.Select(t => new SelectListItem
        {
            Text = t.Name,
            Value = t.TenantId
        }).ToList();
    }

    public async Task<SelectListItem> GetTenantForOrg(string org)
    {
        var organization = await _tenantContext.Tenants
            .FirstOrDefaultAsync(o => o.Name == org);

        if (organization != null)
        {
            return new SelectListItem
            {
                Text = organization.Name,
                Value = organization.TenantId
            };
        }

        return _common;
    }

    public async Task AddTenantAsync(Tenant tenant)
    {
        _tenantContext.Tenants.Add(tenant);
        await _tenantContext.SaveChangesAsync();
    }
}
