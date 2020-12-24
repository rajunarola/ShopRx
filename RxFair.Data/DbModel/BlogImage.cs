using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class BlogImage : EntityWithAudit
    {
        public long BlogId { get; set; }
        [ForeignKey("BlogId")]
        public virtual Blog Blog { get; set; }

        [Required]
        public string ImageName { get; set; }
    }
}
