using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class RewardMoneyMaster : EntityWithAudit
    {
        [Required]
        public long RewardTypeId { get; set; }

        [ForeignKey("RewardTypeId")]
        public virtual RewardTypeMaster RewardTypeMaster { get; set; }

        [Required, StringLength(150)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MinRange { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal MaxRange { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Referral { get; set; }
    }
}
