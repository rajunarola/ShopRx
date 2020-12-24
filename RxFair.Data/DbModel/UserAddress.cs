using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class UserAddress : EntityWithAudit
    {
        [StringLength(256)]
        public string Address1 { get; set; }

        [StringLength(256)]
        public string Address2 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [ForeignKey("StateId")]
        public long? StateId { get; set; }
        public virtual State State { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser User { get; set; }
    }
}
