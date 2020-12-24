using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class Distributor : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string CompanyLogo { get; set; }

        [Required, StringLength(256)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(10)]
        public string ZipCode { get; set; }

        [Required, StringLength(150)]
        public string Address { get; set; }

        public long StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        //ContactPersonInfo

        [Required, StringLength(100)]
        public string ContactName { get; set; }

        [Required, StringLength(256)]
        public string ContactEmail { get; set; }

        [Required, StringLength(20)]
        public string ContactMobile { get; set; }

        [StringLength(100)]
        public string ContactAddress { get; set; }

        public string ContactZipCode { get; set; }

        public long? ContactStateId { get; set; }
        [ForeignKey("ContactStateId")]
        public virtual State ContactState { get; set; }

        public string ContactCity { get; set; }

        //UserId of Distributor admin
        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser DistributorAdminUser { get; set; }

        //Indicates which users are part of Distributor
        [InverseProperty("Distributor")]
        public virtual ICollection<ApplicationUser> DistributorUsers { get; set; }

        public virtual DistributorOrderSetting DistributorOrderSettings { get; set; }
        public virtual DistributorDocumentMaster DistributorDocumentMaster { get; set; }
        public virtual ICollection<DistributorSubscription> DistributorSubscriptions { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
        public virtual ICollection<MedicineMaster> MedicineMasters { get; set; }
        public virtual ICollection<MedicinePriceMaster> MedicinePriceMasters { get; set; }
        public virtual ICollection<UploadedMedicine> UploadedMedicines { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<DistributorMedicine> DistributorMedicines { get; set; }
        public virtual ICollection<WatchList> WatchLists { get; set; }
        public virtual ICollection<DistributorOrder> DistributorOrders { get; set; }
        public virtual ICollection<DistributorOrderCharge> DistributorOrderCharges { get; set; }
        public virtual ICollection<CommissionHistory> CommissionHistories { get; set; }
    }
}
