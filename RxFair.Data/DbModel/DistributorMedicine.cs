using RxFair.Data.DbModel.BaseModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace RxFair.Data.DbModel
{
    public class DistributorMedicine : EntityWithAudit
    {
        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

    }
}
