using System.Security.Claims;
using Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Identity.ClaimProvider;

public class ClaimProvider : IClaimsTransformation
{
    public UserManager<AppUser> _userManager { get; set; }

    public ClaimProvider(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal is not null && principal.Identity.IsAuthenticated)
        {
            var identity = principal.Identity as ClaimsIdentity;
            var user = await _userManager.FindByNameAsync(identity.Name);

            if (user is not null)
            {
                if (user.City is not null)
                {
                    if (!principal.HasClaim(c => c.Type == "City"))
                    {
                        Claim CityClaim = new Claim("city", user.City, ClaimValueTypes.String, "Internal");
                        identity.AddClaim(CityClaim);
                    }
                }
            }
        }

        return principal;
    }
}