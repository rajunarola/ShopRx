using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class PharmacyShippingAddress : EntityWithAudit
    {
        public long PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required]
        public string City { get; set; }

        public long StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [Required]
        public string ZipCode { get; set; }

        public bool IsDefault { get; set; }
    }
}
