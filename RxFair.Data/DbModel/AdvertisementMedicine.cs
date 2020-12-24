using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class AdvertisementMedicine : EntityWithAudit
    {
        public long AdvertisementId { get; set; }
        [ForeignKey("AdvertisementId")]
        public virtual Advertisement Advertisement { get; set; }

        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

        [Required]
        public float DealPrice { get; set; }
    }
}
