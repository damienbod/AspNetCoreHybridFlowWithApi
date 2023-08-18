using System.Security.Claims;
using IdentityModel;

using StsServerIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer;

namespace StsServerIdentity;

public class IdentityWithAdditionalClaimsProfileService : IProfileService
{
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityWithAdditionalClaimsProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var sub = context.Subject.GetSubjectId();

        var user = await _userManager.FindByIdAsync(sub);
        var principal = await _claimsFactory.CreateAsync(user);

        var claims = principal.Claims.ToList();

            claims.Add(new Claim("gender", "unknown"));

        if (user.DataEventRecordsRole == "dataEventRecords.admin")
        {
            claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords.admin"));
            claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords.user"));
            claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords"));
            claims.Add(new Claim(JwtClaimTypes.Scope, "dataEventRecords"));
        }
        else
        {
            claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords.user"));
            claims.Add(new Claim(JwtClaimTypes.Role, "dataEventRecords"));
            claims.Add(new Claim(JwtClaimTypes.Scope, "dataEventRecords"));
        }

        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));

        if (user.IsAdmin)
        {
            claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
        }
        else
        {
            claims.Add(new Claim(JwtClaimTypes.Role, "user"));
        }

        if (user.TwoFactorEnabled)
        {
            claims.Add(new Claim("amr", "mfa"));
        }
        else
        {
            claims.Add(new Claim("amr", "pwd")); ;
        }

        claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var sub = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(sub);
        context.IsActive = user != null;
    }
}