using RxFair.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RxFair.Data.DbModel
{
    public class Cart : EntityWithAudit
    {
        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

        public long PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime CartDate { get; set; }

    }
}
