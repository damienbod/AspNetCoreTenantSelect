using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreSelectTenant;

public static class TenantProvider
{
    private static List<SelectListItem> _tenants = new List<SelectListItem>
    {
        new SelectListItem("Org1", "1"),
        new SelectListItem("Org2", "2"),
        new SelectListItem("Org3", "3")
    };

    public static string GetTenant(string email)
    {
        if (email == "damien_bod@hotmail.com")
            return "Org1";
        else
            return "Org2";
    }

    public static List<SelectListItem> GetAvailableTenants(string email)
    {
        if (email == "damien_bod@hotmail.com")
            return _tenants;
        else
            return new List<SelectListItem> { new SelectListItem("Org1", "1") };
    }
}
