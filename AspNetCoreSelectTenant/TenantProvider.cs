using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreSelectTenant;

public static class TenantProvider
{
    private static SelectListItem _org1 = new("Org1", "7ff95b15-dc21-4ba6-bc92-824856578fc1");
    private static SelectListItem _org2 = new("Org2", "a0958f45-195b-4036-9259-de2f7e594db6");
    private static SelectListItem _org3 = new("Org3", "5698af84-5720-4ff0-bdc3-9d9195314244");

    public static string GetTenantForOrg(string org)
    {
        if (org == "Org1")
            return _org1.Value;
        else if (org == "Org2")
            return _org2.Value;
        else
            return _org3.Value;
    }

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
            return new List<SelectListItem> { _org1, _org2, _org3};
        else if (email == "damienbod@damienbodsharepoint.onmicrosoft.com")
            return new List<SelectListItem> { _org3 };
        else if (email == "damien@damienbod.onmicrosoft.com")
            return new List<SelectListItem> { _org2 };
        
        else
            return new List<SelectListItem> { new SelectListItem("Org1", "1") };
    }
}
