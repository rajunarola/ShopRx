using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("RedeemRequest")]
    public class RedeemRequest : EntityWithAudit
    {
        public long RewardProductId { get; set; }
        [ForeignKey("RewardProductId")]
        public virtual RewardProduct RewardProduct { get; set; }

        public long PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        public bool? IsApprove { get; set; }

        public short DeliveryStatus { get; set; }
    }
}