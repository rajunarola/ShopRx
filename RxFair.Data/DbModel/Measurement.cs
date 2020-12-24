using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("Measurement")]
    public class Measurement : EntityWithAudit
    {
        [Required]
        public short MeasurementType { get; set; }

        [Required]
        public string MeasurementUnit { get; set; }

        [InverseProperty("StrengthMeasurement")] public virtual ICollection<MedicineMaster> StrengthMeasurement { get; set; }

        [InverseProperty("BrandMeasurement")] public virtual ICollection<MedicineMaster> BrandMeasurement { get; set; }

        [InverseProperty("PackageMeasurement")] public virtual ICollection<MedicineMaster> PackageMeasurement { get; set; }

        [InverseProperty("UnitMeasurement")] public virtual ICollection<MedicineMaster> UnitMeasurement { get; set; }

        [InverseProperty("PackageDescriptionCodeMeasurement")] public virtual ICollection<MedicineMaster> PackageDescriptionCodeMeasurement { get; set; }
    }
}
