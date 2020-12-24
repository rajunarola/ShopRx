using RxFair.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RxFair.Data.DbModel
{
   public class CategoryCommission: EntityWithAudit
    {
        [Required]
        public string Name { get; set; }

        public long SubscriptionTypeId { get; set; }
        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal CommissionRate { get; set; } 

    }
}
