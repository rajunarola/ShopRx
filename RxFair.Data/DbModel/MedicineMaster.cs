using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class MedicineMaster : EntityWithAudit
    {
        public MedicineMaster()
        {
            IsDiscontinue = false;
        }
        [Required, Column("NDC_UPC_HRI"), StringLength(11)]
        public string NdcUpcHri { get; set; }

        [StringLength(11), Column("Old_NDC_UPC_HRI")]
        public string OldNdcUpcHri { get; set; }

        [Column("New_NDC_UPC_HRI"), StringLength(11)]
        public string NewNdcUpcHri { get; set; }

        [Column("Drug_Descriptor_Identifier")]
        public int? DrugDescriptorIdentifier { get; set; }

        [StringLength(14), Column("Generic_Product_Identifier")]
        public string GenericProductIdentifier { get; set; }

        [StringLength(8), Column("Generic_Product_Packaging_Code")]
        public string GenericProductPackagingCode { get; set; }

        [StringLength(30), Column("Drug_Name")]
        public string DrugName { get; set; }

        [StringLength(15)]
        public string Strength { get; set; }

        [Column("Strength_Id")] public long? StrengthId { get; set; }
        [ForeignKey("Strength_Id")] public virtual Measurement StrengthMeasurement { get; set; }

        [Column("Dosage_Form_Id")] public long? DosageFormId { get; set; }
        [ForeignKey("Dosage_Form_Id")] public virtual DosageFormMaster DosageFormMaster { get; set; }

        [Column("Brand_Id")] public long? BrandId { get; set; }
        [ForeignKey("Brand_Id")] public virtual Measurement BrandMeasurement { get; set; }

        [Column("Package_Size")]
        public float? PackageSize { get; set; }

        [Column("Package_Size_Id")] public long? PackageSizeId { get; set; }
        [ForeignKey("Package_Size_Id")] public virtual Measurement PackageMeasurement { get; set; }

        [Column("Package_Quantity")]
        public int? PackageQuantity { get; set; }

        [Column("Unit_Size_Id")] public long? UnitSizeId { get; set; }
        [ForeignKey("Unit_Size_Id")] public virtual Measurement UnitMeasurement { get; set; }

        [Column("Package_Description_Code_Id")] public long? PackageDescriptionCodeId { get; set; }
        [ForeignKey("Package_Description_Code_Id")] public virtual Measurement PackageDescriptionCodeMeasurement { get; set; }

        [Column("Manufacturer_Id")] public long? ManufacturerId { get; set; }
        [ForeignKey("Manufacturer_Id")] public virtual ManufacturerMaster ManufacturerMaster { get; set; }

        public string MedicineImage { get; set; }

        public string Description { get; set; }

        [Column("Last_Change_Date")]
        public long? LastChangeDate { get; set; }

        [Column("IsNDC")]
        public bool IsNdc { get; set; }

        public long? DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor DistributorMedicine { get; set; }

        public long? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual MedicineCategoryMaster CategoryMaster { get; set; }

        [StringLength(300)]
        public string Reason { get; set; }

        [StringLength(50)]
        public string Flavour { get; set; }

        public bool IsDelete { get; set; }

        public bool IsDiscontinue { get; set; }

        public virtual ICollection<MedicineImage> MedicineImages { get; set; }
        public virtual ICollection<MedicinePriceMaster> MedicinePriceMasters { get; set; }
        public virtual ICollection<AdvertisementMedicine> AdvertisementMedicines { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<WatchList> WatchLists { get; set; }
        public virtual ICollection<DistributorMedicine> DistributorMedicines { get; set; }
        public virtual ICollection<DistributorOrder> DistributorOrders { get; set; }
    }

    public class MedicineImage : EntityWithAudit
    {
        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

        public string ImageName { get; set; }
    }
}
