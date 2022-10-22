using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IdentityStandaloneMfa;

public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
{
    public AdditionalUserClaimsPrincipalFactory( 
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, 
        IOptions<IdentityOptions> optionsAccessor) 
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity;

        var claims = new List<Claim>();

        if (user.TwoFactorEnabled)
        {
            claims.Add(new Claim("amr", "mfa"));
        }
        else
        {
            claims.Add(new Claim("amr", "pwd")); ;
        }

        identity.AddClaims(claims);
        return principal;
    }
}
