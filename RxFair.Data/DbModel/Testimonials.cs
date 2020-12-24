using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class Testimonials : EntityWithAudit
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Feedback { get; set; }

        public string Image { get; set; }
    }
}
