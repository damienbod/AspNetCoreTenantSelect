using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspNetCoreSelectTenant;

/// <summary>
/// Note, no restrictions are in added. You need to authorize the iss and/or the tid claim after an authentication
/// when only you tenants are allowed to access. 
/// </summary>
public class TenantProvider
{
    public const string TenantOrg1 = "7ff95b15-dc21-4ba6-bc92-824856578fc1";
    public const string TenantOrg2 = "a0958f45-195b-4036-9259-de2f7e594db6";
    public const string TenantOrg3 = "5698af84-5720-4ff0-bdc3-9d9195314244";
    public const string MicrosoftAccount = "9188040d-6c67-4c5b-b112-36a304b66dad";

    private static readonly SelectListItem _org1 = new("Org1", TenantOrg1);
    private static readonly SelectListItem _org2 = new("Org2", TenantOrg2);
    private static readonly SelectListItem _org3 = new("Org3", TenantOrg3);
    private static readonly SelectListItem _common = new("common", "common");

    private static readonly object _lock = new();
    private IDistributedCache _cache;
    private const int cacheExpirationInDays = 1;

    public TenantProvider(IDistributedCache cache)
    {
        _cache = cache;
    }

    public void SetTenant(string email, string org)
    {
        AddToCache(email, GetTenantForOrg(org));
    }

    public SelectListItem GetTenant(string email)
    {
        var org = GetFromCache(email);

        if (org != null)
            return org;

        return _common;
    }

    public List<SelectListItem> GetAvailableTenants()
    {
        return new List<SelectListItem> { _org1, _org2, _org3, _common };
    }

    private SelectListItem GetTenantForOrg(string org)
    {
        if (org == "Org1")
            return _org1;
        else if (org == "Org2")
            return _org2;
        else if (org == "Org3")
            return _org3;

        return _common;
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
