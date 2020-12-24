using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class DocumentMaster : EntityWithAudit
    {
        [Required]
        public int DocumentType { get; set; }

        [Required, StringLength(100)]
        public string DocumentName { get; set; }

        [Required]
        public string DocumentFile { get; set; }

    }

    public class DistributorDocumentMaster : EntityWithAudit
    {
        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        public string ReturnPolicy { get; set; }
        public string LicenseCertificate { get; set; }
        public string Waiver { get; set; }
    }
}
