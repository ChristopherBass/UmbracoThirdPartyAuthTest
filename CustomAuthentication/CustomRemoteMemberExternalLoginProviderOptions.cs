using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Core;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class CustomRemoteMemberExternalLoginProviderOptions : IConfigureNamedOptions<MemberExternalLoginProviderOptions>
{
    public const string SchemeName = "CustomRemote";
    public void Configure(string? name, MemberExternalLoginProviderOptions options)
    {
        if (name != Constants.Security.MemberExternalAuthenticationTypePrefix + SchemeName)
        {
            return;
        }

        Configure(options);
    }

    // This method is based on the documentation:
    // https://our.umbraco.com/documentation/reference/security/auto-linking/#example-for-members
    public void Configure(MemberExternalLoginProviderOptions options)
    {
        options.AutoLinkOptions = new MemberExternalSignInAutoLinkOptions(
            // Must be true for auto-linking to be enabled
            autoLinkExternalAccount: true,

            // Optionally specify the default culture to create
            // the user as. If null it will use the default
            // culture defined in the web.config, or it can
            // be dynamically assigned in the OnAutoLinking
            // callback.
            defaultCulture: null,

            // Optionally specify the default "IsApprove" status. Must be true for auto-linking.
            defaultIsApproved: true,

            // Optionally specify the member type alias. Default is "Member"
            defaultMemberTypeAlias: "Member"
        )
        {
            // Optional callback
            OnAutoLinking = (autoLinkUser, loginInfo) =>
            {
                //Gets called before we have a user, in case you want to add roles or something manually
                var allClaims = loginInfo.Principal.Claims;
                //autoLinkUser.UserName = allClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                //var roles = allClaims.Select(c => c.Type == ClaimTypes.Role).ToList();
                //user set properties like first / last name, etc.
                //user.Roles = new List<IdentityUserRole<string>>() { }
                //user.AddRole("Member"),
            },
            OnExternalLogin = (user, loginInfo) =>
            {
                //Check if we're updating any properties on the user once we've actually logged in


                return true; //returns a boolean indicating if sign in should continue or not.
            }
        };
    }
}