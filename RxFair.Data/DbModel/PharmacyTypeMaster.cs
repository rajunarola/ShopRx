using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class PharmacyTypeMaster : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string PharmacyTypeName { get; set; }

        public virtual ICollection<Pharmacy> Pharmacies { get; set; }
    }
}
