using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("Order")]
    public class Order : EntityWithAudit
    {
        public string UniqueOrder { get; set; }

        public long? PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal OrderSubTotal { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal OrderShippingTotal { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal OrderGrandTotal { get; set; }

        public bool IsOverNight { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool DeliveryStatus { get; set; }

        public string BillingAddress { get; set; }

        public string ShippingAddress { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser OrderCreatedBy { get; set; }

        public virtual ICollection<RewardEarn> RewardEarns { get; set; }
        public virtual ICollection<DistributorOrderCharge> OrderCharges { get; set; }
    }

    [Table("DistributorOrderCharge")]
    public class DistributorOrderCharge : EntityWithAudit
    {
        [Required]
        public long OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public long? DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal SubTotal { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal ShippingTotal { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal GrandTotal { get; set; }

        [Column(TypeName = "decimal(17, 2)")]
        public decimal? OverNightCharge { get; set; }

        public DateTime? CommissionCountDate { get; set; }

        public short OrderStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Column(TypeName = "decimal(17, 2)")]
        public decimal? PaymentAmount { get; set; }

        public string PaymentNote { get; set; }

        public DateTime? ShippingDate { get; set; }

        public string TrackingNo { get; set; }

        public string TrackingLink { get; set; }

        public virtual ICollection<DistributorOrder> DistributorOrders { get; set; }
    }

    [Table("DistributorOrder")]
    public class DistributorOrder : EntityWithAudit
    {
        [Required]
        public long OrderChargeId { get; set; }
        [ForeignKey("OrderChargeId")]
        public virtual DistributorOrderCharge OrderCharge { get; set; }

        public long? DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        [Required]
        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Price { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal TotalPrice { get; set; }

    }
}