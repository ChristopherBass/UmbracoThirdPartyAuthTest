using Microsoft.AspNetCore.Authentication;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Website.Security;
using SiteUmbraco.ContentModels;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

public static class CustomRemoteUmbracoBuilderExtension
{
    public static IUmbracoBuilder AddMemberCustomRemoteAuthentication(this IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<CustomRemoteMemberExternalLoginProviderOptions>();

        builder.AddMemberExternalLogins(logins =>
        {
            logins.AddMemberLogin(
                memberAuthenticationBuilder =>
                {
                    memberAuthenticationBuilder.AddRemoteScheme<CustomRemoteAuthenticationOptions, CustomRemoteAuthenticationHandler>(
                        // The scheme must be set with this method to work for the umbraco members
                        memberAuthenticationBuilder.SchemeForMembers(CustomRemoteMemberExternalLoginProviderOptions.SchemeName),
                        "CustomRemote Auth",
                        options =>
                        {
                            options.CallbackPath = "/auth/signinvalidate";
                            options.Events = new RemoteAuthenticationEvents
                            {
                                OnTicketReceived = context =>
                                {
                                    var AuthInfo = context.HttpContext.RequestServices.GetRequiredService<IPublishedContentQuery>().ContentAtRoot().OfType<WebsiteConfiguration>().FirstOrDefault();

                                    //context.Properties.RedirectUri = AuthInfo?.SsoLoginFallbackRedirectUrl?.Url ?? "/";

                                    return Task.CompletedTask;
                                }
                            };
                        });
                });
        });
        return builder;
    }
}