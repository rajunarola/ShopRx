using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class TimeZoneMaster : EntityWithAudit
    {
        [Required, StringLength(250)]
        public string TimeZoneId { get; set; }

        [Required, StringLength(250)]
        public string DisplayName { get; set; }

        [Required, StringLength(50)]
        public string StandardName { get; set; }

        [Required, StringLength(250)]
        public string DaylightName { get; set; }

        [Required]
        public bool IsDaylight { get; set; }

        [Required, StringLength(10)]
        public string UtcStandardOffset { get; set; }

        [Required, StringLength(10)]
        public string UtcDaylightOffset { get; set; }

        public virtual ICollection<DistributorOrderSetting> DistributorOrderSettings { get; set; }
    }
}
