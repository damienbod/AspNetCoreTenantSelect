﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AspNetCoreSelectTenant;

public class TenantHandler : AuthorizationHandler<TenantRequirement>
{
    private const string TenantOrg1 = "7ff95b15-dc21-4ba6-bc92-824856578fc1";
    private const string TenantOrg2 = "a0958f45-195b-4036-9259-de2f7e594db6";
    private const string TenantOrg3 = "5698af84-5720-4ff0-bdc3-9d9195314244";
    private const string MicrosoftAccount = "9188040d-6c67-4c5b-b112-36a304b66dad";

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

    private static bool IsTenantValid( Claim? tenantId)
    {
        if (tenantId == null)
        {
            return false;
        }

        if (tenantId.Value == TenantOrg1
            || tenantId.Value == TenantOrg2
            || tenantId.Value == TenantOrg3
            || tenantId.Value == MicrosoftAccount)
        {
            return true;
        }

        return false;
    }

}
