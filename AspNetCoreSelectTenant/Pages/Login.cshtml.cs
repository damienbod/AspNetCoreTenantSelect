using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreSelectTenant.Pages;

public class LoginModel : PageModel
{
    public string Domain { get; set; } = string.Empty;

    public void OnGet()
    {
    }
}