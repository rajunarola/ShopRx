using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class UploadedMedicine : EntityWithAudit
    {
        [Required, StringLength(11)]
        public string Ndc { get; set; }

        [StringLength(11)]
        public string Upc { get; set; }

        [StringLength(100)]
        public string MedicineName { get; set; }

        public string MedicineImage { get; set; }

        [StringLength(50)]
        public string Strength { get; set; }

        [StringLength(4)]
        public string DosageForm { get; set; }

        [StringLength(4)]
        public string Brand { get; set; }

        [Column("PackageSize")]
        public float? PackageSize { get; set; }

        [Column("Unit"), StringLength(1)]
        public string Unit { get; set; }

        public string Manufacturer { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor DistributorMedicine { get; set; }

        public bool? IsApproved { get; set; }

        [StringLength(300)]
        public string Reason { get; set; }

        [StringLength(50)]
        public string Flavour { get; set; }
    }
}