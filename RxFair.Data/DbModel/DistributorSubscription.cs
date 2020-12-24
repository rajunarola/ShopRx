using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class DistributorSubscription : EntityWithAudit
    {
        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

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

        public bool IsUpgraded { get; set; }

        public bool IsPayment { get; set; }

        public bool IsExpire { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PayPalTransactionId { get; set; }

        public virtual ICollection<DistributorSubscriptionHistory> DistributorSubscriptionHistories { get; set; }
    }
}
