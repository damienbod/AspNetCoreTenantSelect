using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AspNetCoreSelectTenant.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty]
    public string Domain { get; set; } = string.Empty;

    [BindProperty]
    public string TenantId { get; set; } = string.Empty;

    [BindProperty]
    public List<string> RolesInTenant { get; set; } = new List<string>();

    [BindProperty]
    public string AppTenantName { get; set; } = string.Empty;

    [BindProperty]
    public List<SelectListItem> AvailableAppTenants { get; set; } = new List<SelectListItem>();

    public void OnGet()
    {
        var name = User.Identity!.Name;

        if (name != null)
        {
            AvailableAppTenants = TenantProvider.GetAvailableTenants(name);
            AppTenantName = TenantProvider.GetTenant(name);

            List<Claim> roleClaims = HttpContext.User.FindAll(ClaimTypes.Role).ToList();

            foreach (var role in roleClaims)
            {
                RolesInTenant.Add(role.Value);
            }

            TenantId = HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
        }
    }

    /// <summary>
    /// Only works from a direct GET, not a post or a redirect
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public IActionResult OnGetSignIn([FromQuery]string domain)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/"},
                OpenIdConnectDefaults.AuthenticationScheme);
    }

    //public IActionResult OnPostSignIn()
    //{
    //    return Unauthorized();
    //    return Redirect("Login?handler=SignIn");
    //    //return Challenge(new AuthenticationProperties { RedirectUri = "/" },
    //    //        OpenIdConnectDefaults.AuthenticationScheme);
    //}
}