using AspNetCoreSelectTenant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;

services.AddDistributedMemoryCache();

services.AddTransient<TenantProvider>();

services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
    options.Events.OnTokenValidated = async context =>
    {
        await existingOnTokenValidatedHandler(context);

        if (context.Principal != null)
        {
            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
        }
    };
});

WebApplication? app = null;

services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Prompt = "select_account";
    
    var redirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
    options.Events.OnRedirectToIdentityProvider = async context =>
    {
        if(app != null)
        {
            var tenantProvider = app.Services.GetRequiredService<TenantProvider>();
            var email = context.HttpContext!.User.Identity!.Name;
            if (email != null)
            {
                var tenant = tenantProvider.GetTenant(email);
                var address = context.ProtocolMessage.IssuerAddress.Replace("common", tenant.Value);
                context.ProtocolMessage.IssuerAddress = address;
            }
        }

        await redirectToIdentityProvider(context);
    };
});

services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSecurityHeaders(SecurityHeadersDefinitions
    .GetHeaderPolicyCollection(env.IsDevelopment()));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
