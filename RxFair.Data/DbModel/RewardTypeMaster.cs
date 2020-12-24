using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class RewardTypeMaster : EntityWithAudit
    {
        [StringLength(50)]
        public string Type { get; set; }

        public virtual ICollection<RewardMoneyMaster> RewardMoneyMasters { get; set; }
        public virtual ICollection<RewardEarn> RewardEarns { get; set; }
    }

}
