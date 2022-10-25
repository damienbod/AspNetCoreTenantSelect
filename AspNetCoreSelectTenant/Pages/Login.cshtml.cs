using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSelectTenant.Pages;

public class LoginModel : PageModel
{
    public string Domain { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        string redirect = Url.Content("~/")!;

        return Challenge(new AuthenticationProperties { RedirectUri = redirect });
    }
}