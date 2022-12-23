using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AspNetCoreSelectTenant.Tenants;

public class TenantProviderCache
{
    private static readonly SelectListItem _common = new("common", "common");

    private static readonly object _lock = new();
    private readonly IDistributedCache _cache;
    private const int cacheExpirationInDays = 1;

    public TenantProviderCache(IDistributedCache cache)
    {
        _cache = cache;
    }

    public void SetTenant(string email, SelectListItem org)
    {
        AddToCache(email, org);
    }

    public SelectListItem GetTenant(string email)
    {
        var org = GetFromCache(email);

        if (org != null)
            return org;

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
