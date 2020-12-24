using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class Pharmacy : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string PharmacyName { get; set; }

        [Required, StringLength(50)]
        public string LicenseNumber { get; set; }

        [Required, StringLength(50)]
        public DateTime LicenseExpires { get; set; }

        [Required]
        public string LicenseFile { get; set; }

        [Required, StringLength(20)]
        public string DeaNumber { get; set; }

        [Required,StringLength(50)]
        public DateTime DeaExpires { get; set; }

        [Required]
        public string DeaFile { get; set; }

        [Required, StringLength(50)]
        public string NpiNumber { get; set; }

        public string ReferCode { get; set; }

        [DefaultValue(1)]
        public int Status { get; set; }

        public long PharmacyTypeId { get; set; }

        [ForeignKey("PharmacyTypeId")]
        public virtual PharmacyTypeMaster PharmacyTypeMaster { get; set; }

        public long PharmacySystemId { get; set; }
        [ForeignKey("PharmacySystemId")]
        public virtual PharmacySystemMaster PharmacySystemMaster { get; set; }

        //UserId of pharmacy admin
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser PharmacyAdminUser { get; set; }

        //Indicates which users are part of pharmacy
        [InverseProperty("Pharmacy")]
        public virtual ICollection<ApplicationUser> PharmacyUsers { get; set; }

        public virtual ICollection<PharmacyBillingAddress> BillingAddresses { get; set; }
        public virtual ICollection<PharmacyShippingAddress> ShippingAddresses { get; set; }
        public virtual ICollection<RewardEarn> RewardEarns { get; set; }
        public virtual ICollection<RedeemRequest> RedeemRequests { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<WatchList> WatchLists { get; set; }
    }

}
