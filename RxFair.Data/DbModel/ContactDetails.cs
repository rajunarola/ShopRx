using System.ComponentModel.DataAnnotations;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class ContactDetails : EntityWithAudit
    {
        [Required, StringLength(256)]
        public string Email { get; set; }

        [Required, StringLength(150)]
        public string Address { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        [Required, StringLength(100)]
        public string State { get; set; }

        [Required, StringLength(10)]
        public string ZipCode { get; set; }

        [Required, StringLength(20)]
        public string Telephone { get; set; }

        [StringLength(15)]
        public string Fax { get; set; }

    }
}
