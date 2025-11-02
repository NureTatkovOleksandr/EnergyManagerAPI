using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyManagerCore.Extentions
{
    using System.Security.Claims;

    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                           ?? user.FindFirst("sub"); // или другой claim, если вы используете JWT

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim not found.");

            return int.Parse(userIdClaim.Value);
        }
    }

}
