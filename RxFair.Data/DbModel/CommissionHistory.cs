using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("CommissionHistory")]
    public class CommissionHistory : EntityWithAudit
    {
        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal CommissionAmount { get; set; }

        public short CommissionStatus { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }
    }

    [Table("Invoice")]
    public class Invoice : EntityWithAudit
    {
        public long CommissionId { get; set; }
        [ForeignKey("CommissionId")]
        public virtual CommissionHistory CommissionHistory { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Amount { get; set; }

        public short InvoiceStatus { get; set; }

        public virtual  ICollection<InvoicePayment> InvoicePayments { get; set; }
    }

    [Table("InvoicePayment")]
    public class InvoicePayment : EntityWithAudit
    {
        public long InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal PaymentAmount { get; set; }

        public short PaymentStatus { get; set; }

        public string PaidBy { get; set; }
    }
}