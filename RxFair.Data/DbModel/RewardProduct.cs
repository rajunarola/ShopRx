using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("RewardProduct")]
    public class RewardProduct : EntityWithAudit
    {
        [Required, Column(TypeName = "varchar(100)")]
        public string ProductName { get; set; }

        [Required, Column(TypeName = "varchar(100)")]
        public string ProductImage { get; set; }

        [Required, Column(TypeName = "decimal(17, 2)")]
        public decimal Redeem { get; set; }

        public string Description { get; set; }

        public virtual ICollection<RedeemRequest> RedeemRequests { get; set; }
    }
}
