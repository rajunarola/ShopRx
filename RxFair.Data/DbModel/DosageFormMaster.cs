using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class DosageFormMaster : EntityWithAudit
    {
        [Required, StringLength(150)]
        public string DosageForm { get; set; }

        public virtual ICollection<MedicineMaster> MedicineMasters { get; set; }
    }
}
