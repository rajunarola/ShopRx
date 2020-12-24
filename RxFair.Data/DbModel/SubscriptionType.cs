using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class SubscriptionType : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string SubscriptionTypeName { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal ChargedMonthly { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal SubscriptionCharge { get; set; }

        public string Description { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Brand { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Generic { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Otc { get; set; }

        public virtual ICollection<DistributorSubscription> DistributorSubscriptions { get; set; }
        public virtual ICollection<SubscriptionTypeHistory> SubscriptionTypeHistories { get; set; }
    }
}
