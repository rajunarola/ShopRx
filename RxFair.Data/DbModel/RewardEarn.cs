using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("RewardEarn")]
    public class RewardEarn : EntityWithAudit
    {
        [Required]
        public long RewardTypeId { get; set; }

        [ForeignKey("RewardTypeId")]
        public virtual RewardTypeMaster RewardTypeMaster { get; set; }

        public long? PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        public long? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public string RewardBy { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal RewardMoney { get; set; }
    }
}