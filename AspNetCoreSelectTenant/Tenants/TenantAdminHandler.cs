using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Tenants;

public class TenantAdminHandler : AuthorizationHandler<TenantAdminRequirement>
{
    private readonly string tenantHomeId;

    public TenantAdminHandler(IConfiguration configuration)
    {
        var tid = configuration["AdminHomeTenant"] 
            ?? throw new Exception("AdminHomeTenant configuration not set");

        tenantHomeId = tid;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAdminRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var tenantId = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/identity/claims/tenantid");
        
        var role = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            && t.Value == "tenant-admin");


        if (IsTenantValid(tenantId) && role != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool IsTenantValid(Claim? tenantId)
    {
        if (tenantId == null)
        {
            return false;
        }

        if (tenantId.Value == tenantHomeId)
        {
            return true;
        }

        return false;
    }

}
