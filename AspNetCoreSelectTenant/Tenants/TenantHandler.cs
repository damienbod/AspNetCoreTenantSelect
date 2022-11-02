using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Tenants;

public class TenantHandler : AuthorizationHandler<TenantRequirement>
{
    private readonly TenantContext _tenantContext;

    public TenantHandler(TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    //private const string TenantOrg1 = "7ff95b15-dc21-4ba6-bc92-824856578fc1";
    //private const string TenantOrg2 = "a0958f45-195b-4036-9259-de2f7e594db6";
    //private const string TenantOrg3 = "5698af84-5720-4ff0-bdc3-9d9195314244";
    //private const string MicrosoftAccount = "9188040d-6c67-4c5b-b112-36a304b66dad";

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        var tenantId = context.User.Claims.FirstOrDefault(t => t.Type == "http://schemas.microsoft.com/identity/claims/tenantid");

        if (IsTenantValid(tenantId))
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

        var tenantExists = _tenantContext.Tenants.Any(t => t.TenantId == tenantId.Value);

        if (tenantExists)
        {
            return true;
        }

        return false;
    }

}
