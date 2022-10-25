using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSelectTenant;

[AllowAnonymous]
[Route("[controller]")]
public class CustomAccountController : Controller
{
    private readonly IConfiguration _configuration;

    public CustomAccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("SignIn")]
    public IActionResult SignInGet([FromQuery] string redirectUri)
    {
        string redirect;
        if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
        {
            redirect = redirectUri;
        }
        else
        {
            redirect = Url.Content("~/")!;
        }

        return Challenge(new AuthenticationProperties { RedirectUri = redirect });
    }

    [HttpPost("SignIn")]
    public IActionResult SignInPost([FromForm] string domain, [FromQuery] string redirectUri)
    {
        string redirect;
        if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
        {
            redirect = redirectUri;
        }
        else
        {
            redirect = Url.Content("~/")!;
        }

        return Challenge(new AuthenticationProperties { RedirectUri = redirect });
    }
}