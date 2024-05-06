using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using SiteUmbraco.ContentModels;

[Route("auth")]
public class AuthenticationController : RenderController
{
    public AuthenticationController(ILogger<AuthenticationController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor)
        : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
    }

    //Sign-in is handled by the CustomRemoteAuthenticationHandler, which is set in CustomRemoteUmbracoBuilderExtension.cs
    [HttpGet("signin")]
    public IActionResult SignInHandler()
    {
        return Challenge("UmbracoMembers.CustomRemote");
    }

    [HttpGet("signout")]
    public IActionResult SignOutHandler()
    {
        var AuthInfo = UmbracoContext.Content?.GetAtRoot().OfType<WebsiteConfiguration>().FirstOrDefault();
        var properties = new AuthenticationProperties
        {
            RedirectUri = AuthInfo?.SsoLogoutReturnLocation?.Url ?? "/"
        };
        return SignOut(properties);
    }
}