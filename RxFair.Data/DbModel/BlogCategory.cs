using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
   public class BlogCategory: EntityWithAudit
    {
        [Required]
        public string CategoryName { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}
