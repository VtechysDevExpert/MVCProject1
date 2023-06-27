using System.Security.Claims;
using System.Security.Principal;

namespace eCommerce.Shared.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserEmail(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Email");

            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserPicture(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Picture");

            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
