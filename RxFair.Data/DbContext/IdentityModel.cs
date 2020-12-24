using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using RxFair.Data.DbModel;

namespace RxFair.Data.DbContext
{
    public class UserRole : IdentityUserRole<long> { }

    public class UserClaim : IdentityUserClaim<long> { }

    public class UserLogin : IdentityUserLogin<long> { }

    public class Role : IdentityRole<long>
    {
        public Role()
        {
            DisplayRoleName = "";
        }
        public string DisplayRoleName { get; set; }

        public virtual ICollection<RolesModuleAccess> RolesModuleAccesses { get; set; }
    }

    public class ApplicationUser : IdentityUser<long>
    {
        public ApplicationUser()
        {
            UserProfileImage = "";
        }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [NotMapped] public string FullName => $@"{FirstName} {LastName}";

        [StringLength(30)]
        public string JobTitle { get; set; }

        [StringLength(100)]
        public string UserProfileImage { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string FaxNumber { get; set; }

        [DefaultValue(false)]
        public bool IsActive { get; set; }

        [ForeignKey("Pharmacy")]
        public long? PharmacyId { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }

        [InverseProperty("PharmacyAdminUser")]
        public virtual ICollection<Pharmacy> PharmacyOwner { get; set; }

        [ForeignKey("Distributor")]
        public long? DistributorId { get; set; }
        public virtual Distributor Distributor { get; set; }

        public DateTime? LastLogin { get; set; }

        [InverseProperty("DistributorAdminUser")]
        public virtual ICollection<Distributor> DistributorOwner { get; set; }

        public virtual ICollection<UserAddress> UserAddress { get; set; }

        public virtual ICollection<Blog> AuthorBlogs { get; set; }

    }
}
