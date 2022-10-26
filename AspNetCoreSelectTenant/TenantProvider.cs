using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspNetCoreSelectTenant;

public class TenantProvider
{
    private static SelectListItem _org1 = new("Org1", "7ff95b15-dc21-4ba6-bc92-824856578fc1");
    private static SelectListItem _org2 = new("Org2", "a0958f45-195b-4036-9259-de2f7e594db6");
    private static SelectListItem _org3 = new("Org3", "5698af84-5720-4ff0-bdc3-9d9195314244");

    private static readonly object _lock = new();
    private IDistributedCache _cache;
    private readonly IHttpContextAccessor _context;
    private const int cacheExpirationInDays = 1;

    public TenantProvider(IDistributedCache cache, IHttpContextAccessor context)
    {
        _cache = cache;
        _context = context;
    }

    public void SetTenant(string email, string org)
    {
        AddToCache(email, GetTenantForOrg(org));
    }

    public string GetTenant()
    {
        var userEmail = _context.HttpContext!.User.Identity!.Name;
        if(userEmail != null)
        {
            var org = GetFromCache(userEmail);

            if (org != null)
                return org.Value;
        }

        return "common";
    }

    public SelectListItem GetTenant(string email)
    {
        var org = GetFromCache(email);

        if (org != null)
            return org;

        return _org1;
    }

    public List<SelectListItem> GetAvailableTenants(string email)
    {
        if (email == "damien_bod@hotmail.com")
            return new List<SelectListItem> { _org1, _org2, _org3};
        else if (email == "damienbod@damienbodsharepoint.onmicrosoft.com")
            return new List<SelectListItem> { _org3 };
        else if (email == "damien@damienbod.onmicrosoft.com")
            return new List<SelectListItem> { _org2 };
        
        else
            return new List<SelectListItem> { _org1 };
    }

    private SelectListItem GetTenantForOrg(string org)
    {
        if (org == "Org1")
            return _org1;
        else if (org == "Org2")
            return _org2;
        else
            return _org3;
    }

    private void AddToCache(string key, SelectListItem userActiveOrg)
    {
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(cacheExpirationInDays));

        lock (_lock)
        {
            _cache.SetString(key, JsonSerializer.Serialize(userActiveOrg), options);
        }
    }

    private SelectListItem? GetFromCache(string key)
    {
        var item = _cache.GetString(key);
        if (item != null)
        {
            return JsonSerializer.Deserialize<SelectListItem>(item);
        }

        return null;
    }
}
