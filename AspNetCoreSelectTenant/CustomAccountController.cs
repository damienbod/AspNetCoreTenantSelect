using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreSelectTenant;

[Route("[controller]")]
public class CustomAccountController : Controller
{
    private readonly IConfiguration _configuration;

    public CustomAccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //[HttpGet("SignInGet")]
    //public IActionResult SignInGet([FromQuery] string redirectUri)
    //{
    //    string redirect;
    //    if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
    //    {
    //        redirect = redirectUri;
    //    }
    //    else
    //    {
    //        redirect = Url.Content("~/")!;
    //    }

    //    return Challenge(new AuthenticationProperties { RedirectUri = redirect }, OpenIdConnectDefaults.AuthenticationScheme);
    //}

    [HttpPost("SignIn")]
    public IActionResult SignIn([FromForm] string domain, [FromForm] string redirectUri)
    {
        // TODO handle the domain
        string redirect;
        if (!string.IsNullOrEmpty(redirectUri) && Url.IsLocalUrl(redirectUri))
        {
            redirect = redirectUri;
        }
        else
        {
            redirect = Url.Content("~/")!;
        }

        return Challenge(new AuthenticationProperties { RedirectUri = redirect }, OpenIdConnectDefaults.AuthenticationScheme);
    
        //return Redirect($"SignInGet?redirect={redirect}");
    }
}