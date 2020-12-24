using RxFair.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RxFair.Data.DbModel
{
   public class OrderWiseReward: EntityWithAudit
    {
        public long OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public long PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public virtual Pharmacy Pharmacy { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Reward { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal RewardPoints { get; set; }

    }
}
