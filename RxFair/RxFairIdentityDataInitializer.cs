using Microsoft.AspNetCore.Identity;
using RxFair.Data.DbContext;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RxFair
{
    public static class RxFairIdentityDataInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser
            {
                FirstName = "Simon",
                LastName = "Ho",
                JobTitle = "RxFair",
                UserName = "sim91h@gmail.com",
                UserProfileImage = "",
                Mobile = "+1 999-999-9999",
                FaxNumber = "+1 999-999-9999",
                NormalizedUserName = "System Super Admin",
                Email = "sim91h@gmail.com",
                NormalizedEmail = "sim91h@gmail.com",
                EmailConfirmed = true,
                IsActive = true
            };
            if (userManager.FindByEmailAsync(user.UserName).Result == null)
            {
                var result = userManager.CreateAsync(user, "Sim91h@").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "SystemSuperAdmin").Wait();
                }
            }


            var developerUser = new ApplicationUser
            {
                FirstName = "Anc",
                LastName = "Narola",
                JobTitle = "Narola",
                UserName = "cgankit@gmail.com",
                UserProfileImage = "",
                Mobile = "+1 999-999-9999",
                FaxNumber = "+1 999-999-9999",
                NormalizedUserName = "Narola Admin",
                Email = "cgankit@gmail.com",
                NormalizedEmail = "cgankit@gmail.com",
                EmailConfirmed = true,
                IsActive = true
            };
            if (userManager.FindByEmailAsync(developerUser.UserName).Result != null) return;
            var developeResult = userManager.CreateAsync(developerUser, "P@ssword123").Result;

            if (developeResult.Succeeded)
            {
                userManager.AddToRoleAsync(developerUser, "Developer").Wait();
            }
        }

        private static void SeedRoles(RoleManager<Role> roleManager)
        {
            #region User Roles
            Dictionary<string, string> normalizedName = new Dictionary<string, string>
            {
                { "SystemSuperAdmin", "System Super Admin"},
                { "SystemAdmin", "System Admin"},
                { "FinanceManager", "Finance Manager"},
                { "DataManager", "Data Manager"},
                { "CustomerSupportForDistributor", "Customer Support For Distributor"},
                { "CustomerSupportForPharmacy", "Customer Support For Pharmacy"},
                { "DistributorPrimaryAdmin", "Distributor Primary Admin"},
                { "DistributorAdmin", "Distributor Admin"},
                { "DistributorStaff", "Distributor Staff"},
                { "PharmacyPrimaryAdmin", "Pharmacy Primary Admin"},
                { "PharmacyAdmin", "Pharmacy Admin"},
                { "PharmacyStaff", "Pharmacy Staff"},
                { "Developer", "Narola Admin"}
            };

            var existrolesList = roleManager.Roles.Select(x => x.Name).ToList();
            if (existrolesList.Any())
            {
                var notExirst = normalizedName.Keys.Except(existrolesList);
                foreach (var notRole in notExirst)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == notRole).Value;
                    var roleResult = roleManager.CreateAsync(new Role { Name = notRole, NormalizedName = normalized, DisplayRoleName = normalized }).Result;
                }
            }
            else
            {
                foreach (var objRole in normalizedName.Keys)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == objRole).Value;
                    IdentityResult roleResult = roleManager.CreateAsync(new Role { Name = objRole, NormalizedName = normalized, DisplayRoleName = normalized }).Result;
                }
            }
            #endregion
        }

        public static long GetTimeZoneId(string currentTimeZone, string connection)
        {
            using (var sqlCon = new SqlConnection(connection))
            {
                var query = $@"EXEC GetCurrentTimeZone '{currentTimeZone}';";
                sqlCon.Open();
                var sqlCmd = new SqlCommand(query, sqlCon);
                var result = sqlCmd.ExecuteScalar();
                sqlCon.Close();
                return (long?)result ?? 0;
            }
        }
    }
}
