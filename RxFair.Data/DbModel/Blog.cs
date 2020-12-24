using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class Blog : EntityWithAudit
    {
        [Required]
        public string Title { get; set; }

        [Required, StringLength(50)]
        public string InternalName { get; set; }

        public long BlogCategoryId { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual BlogCategory BlogCategory { get; set; }

        public long AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser Author { get; set; }

        [Required]
        public DateTime BlogDate { get; set; }

        [NotMapped]
        public string Tags => string.Join(",", BlogTags.Select(x => x.TagName).ToList());

        [Required]
        public string Descriptions { get; set; }

        public string RelatedBlogs { get; set; }

        public virtual ICollection<BlogImage> BlogImages { get; set; }
        public virtual ICollection<BlogTag> BlogTags { get; set; }
    }
}
