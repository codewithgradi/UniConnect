using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UniConnect.Domain.Entities;

namespace UniConnect.Infrastructure.Identity;

public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public CustomClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim("user_type", user.UserType.ToString()));
        identity.AddClaim(new Claim("verification_status", user.VerificationStatus.ToString()));
        identity.AddClaim(new Claim("is_active", user.IsActive.ToString().ToLower()));

        return identity;
    }
}