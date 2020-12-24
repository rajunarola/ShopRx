using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Dto.Enum;
using RxFair.Utility.Common;

namespace RxFair
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, Role>
    {
        public ClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().FirstOrDefault()?.Value ?? "";
            string pharmacyId = (role == UserRoles.PharmacyPrimaryAdmin) ? user.Pharmacy.Id.ToString() : user.PharmacyId?.ToString() ?? "";
            string distributorId = (role == UserRoles.DistributorPrimaryAdmin) ? user.Distributor.Id.ToString() : user.DistributorId?.ToString() ?? "";
            long subscriptionTypeId = (role.Contains("Distributor") ? user.Distributor?.DistributorSubscriptions?.FirstOrDefault(x => x.IsActive)?.SubscriptionTypeId ?? 0 : 0);
            bool IsSubscriptionExpired = (role == UserRoles.DistributorPrimaryAdmin)
                ? user.Distributor?.DistributorSubscriptions.Count(x => x.IsActive && x.EndDate.Value.Date > DateTime.Now.Date) == 0 : false;
            var claims = new List<Claim>()
            {
                new Claim("UserRole", role),
                new Claim("DisplayUserRole", CommonMethod.GetDisplayUserRole(role)),
                new Claim("UserRoleGroup", CommonMethod.GetUserGroupName(role)),
                new Claim("IsSubscriptionExpired", Convert.ToString(IsSubscriptionExpired)),
                new Claim("SubscriptionTypeId", Convert.ToString(subscriptionTypeId)),
                new Claim("UserId", user.Id.ToString() ?? ""),
                new Claim("PharmacyId", pharmacyId),
                new Claim("DistributorId", distributorId),
                new Claim("LastName", user.LastName ??"" ),
                new Claim("FullName", user.FullName ??"" ),
                new Claim("UserProfileImage", $@"\{FilePathList.UserProfile}\{user.UserProfileImage}" ?? @"/UploadFile/UserProfile/user.png"),
            };
            identity.AddClaims(claims);
            return identity;
        }
    }
}
