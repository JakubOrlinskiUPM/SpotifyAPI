using System;
using System.Security.Claims;
using IdentityModel;

namespace SpotifyAPI.Core.Identity.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetId (this ClaimsPrincipal claimsPrincipal)
        {
            if (Guid.TryParse(
                claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier),
                out var idFromNameIdentifierClaim
            )) return idFromNameIdentifierClaim;

            if (Guid.TryParse(claimsPrincipal.FindFirstValue(JwtClaimTypes.Subject), out var idFromSubClaim))
                return idFromSubClaim;

            return null;
        }
    }
}