using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using SiteUmbraco.ContentModels;

public class CustomRemoteAuthenticationHandler : RemoteAuthenticationHandler<CustomRemoteAuthenticationOptions>
{
    public IPublishedContentQuery PublishedContentQuery { get; }

    public CustomRemoteAuthenticationHandler(IOptionsMonitor<CustomRemoteAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IPublishedContentQuery publishedContentQuery)
        : base(options, logger, encoder)
    {
        PublishedContentQuery = publishedContentQuery;
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var token = Request.Query["token"].ToString();

        // Send SOAP request to third-party service with the token and get user data
        // ...

        // Create claims for the user data
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "exampleuser@test.com"),
            new Claim(ClaimTypes.Name, "Test User"),  //Display name 
            new Claim(ClaimTypes.Email, "exampleuser@test.com") //This maps the user to log in as.
            // Add other claims as needed
            //new Claim(ClaimTypes.Role, "Role1"),
            //new Claim(ClaimTypes.Role, "Role2"),
        };

        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return HandleRequestResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (Request.Path != Options.CallbackPath)
        {
            string redirectUri = string.IsNullOrEmpty(properties.RedirectUri) ? OriginalPathBase + OriginalPath : properties.RedirectUri;

            var AuthInfo = PublishedContentQuery.ContentAtRoot().OfType<WebsiteConfiguration>().FirstOrDefault();

            if (AuthInfo?.SsoLoginUrl?.Url == null)
            {
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return Task.CompletedTask;
            }

            var challengeUrl = AuthInfo?.SsoLoginUrl?.Url + "?returnurl =" + Uri.EscapeDataString(redirectUri); //Needs to be absolute, isn't.

            Response.Redirect(challengeUrl);

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}