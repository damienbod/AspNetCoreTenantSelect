using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Tenants;

public class TenantAdminHandler : AuthorizationHandler<TenantAdminRequirement>
{
    private const string TenantOrg1 = "7ff95b15-dc21-4ba6-bc92-824856578fc1";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantAdminRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var tenantId = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/identity/claims/tenantid");
        
        var role = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/identity/claims/role" 
            && t.Value == "tenant-admin");


        if (IsTenantValid(tenantId) && role != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool IsTenantValid(Claim? tenantId)
    {
        if (tenantId == null)
        {
            return false;
        }

        if (tenantId.Value == TenantOrg1)
        {
            return true;
        }

        return false;
    }

}
