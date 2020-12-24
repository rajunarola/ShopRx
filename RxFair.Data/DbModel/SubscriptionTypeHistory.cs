using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("SubscriptionTypeHistory")]
    public class SubscriptionTypeHistory : EntityWithAudit
    {
        public long SubscriptionTypeId { get; set; }
        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal ChargedMonthly { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal SubscriptionCharge { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Brand { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Generic { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Otc { get; set; }
    }
}