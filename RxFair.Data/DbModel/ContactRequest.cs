using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class ContactRequest : EntityWithAudit
    {
        [Required, StringLength(60)]
        public string Name { get; set; }

        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required, StringLength(256)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
