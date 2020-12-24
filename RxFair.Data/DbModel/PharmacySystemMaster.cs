using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class PharmacySystemMaster : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string PharmacySystemName { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    }
}
