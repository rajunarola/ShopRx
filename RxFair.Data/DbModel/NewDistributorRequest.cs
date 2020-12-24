using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class NewDistributorRequest : EntityWithAudit
    {
        [Required, StringLength(50)]
        public string CompanyName { get; set; }

        [Required, StringLength(256)]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [Required, StringLength(10)]
        public string ZipCode { get; set; }

        [Required, StringLength(150)]
        public string Address { get; set; }

        public long StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [Required, StringLength(100)]
        public string City { get; set; }

        //ContactPersonInfo

        [Required, StringLength(100)]
        public string ContactName { get; set; }

        [Required, StringLength(256)]
        public string ContactEmail { get; set; }

        [Required, StringLength(20)]
        public string ContactMobile { get; set; }

        [StringLength(100)]
        public string ContactAddress { get; set; }

        public string ContactZipCode { get; set; }

        public long? ContactStateId { get; set; }
        [ForeignKey("ContactStateId")]
        public virtual State ContactState { get; set; }

        public string ContactCity { get; set; }
    }
}
