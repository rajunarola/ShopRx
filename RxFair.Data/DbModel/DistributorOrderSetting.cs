using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class DistributorOrderSetting : EntityWithAudit
    {
        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        public long TimeZoneId { get; set; }
        [ForeignKey("TimeZoneId")]
        public virtual TimeZoneMaster TimeZoneMaster { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal MinOrderAmount { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal ShippingCharge { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal OverNightAmount { get; set; }

        public bool ServiceDayMonday { get; set; }
        public bool ServiceDayTuesday { get; set; }
        public bool ServiceDayWednesday { get; set; }
        public bool ServiceDayThursday { get; set; }
        public bool ServiceDayFriday { get; set; }
        public bool ServiceDaySaturday { get; set; }
        public bool ServiceDaySunday { get; set; }

        public TimeSpan? MondayCutOffTime { get; set; }
        public TimeSpan? TuesdayCutOffTime { get; set; }
        public TimeSpan? WednesdayCutOffTime { get; set; }
        public TimeSpan? ThursdayCutOffTime { get; set; }
        public TimeSpan? FridayCutOffTime { get; set; }
        public TimeSpan? SaturdayCutOffTime { get; set; }
        public TimeSpan? SundayCutOffTime { get; set; }
    }

}