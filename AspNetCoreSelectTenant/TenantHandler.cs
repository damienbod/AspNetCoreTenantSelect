using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreSelectTenant;

public class TenantHandler : AuthorizationHandler<TenantRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var client_id = context.User.Claims.FirstOrDefault(t => t.Type == "client_id");
        var tenantId = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/identity/claims/tenantid");

        if (IsTenantValid(tenantId))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private bool IsTenantValid( Claim? tenantId)
    {
        if (tenantId == null)
        {
            return false;
        }

        if (tenantId.Value == TenantProvider.TenantOrg1
            || tenantId.Value == TenantProvider.TenantOrg2
            || tenantId.Value == TenantProvider.TenantOrg3)
        {
            return true;
        }

        return false;
    }

}
