using System;
using System.Security.Claims;

namespace RxFair.Service.Exceptions
{
    public static class IdentityExtension
    {
        /// <summary>
        /// Get Logged in User Id
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>Return User Id of Current Logged In User</returns>
        public static long GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));
            return ConvertToLong(principal.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private static long ConvertToLong(string userId)
        {
            return Convert.ToInt64(userId);
        }
    }
}
