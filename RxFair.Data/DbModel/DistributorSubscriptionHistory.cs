using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("DistributorSubscriptionHistory")]
    public class DistributorSubscriptionHistory : EntityWithAudit
    {
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

        public bool IsPayment { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PayPalTransactionId { get; set; }
    }
}