using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSelectTenant.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    public string Domain { get; set; } = string.Empty;

    public void OnGet()
    {
    }
}