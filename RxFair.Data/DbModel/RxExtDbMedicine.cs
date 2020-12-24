using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RxFair.Data.DbModel
{
    public class RxExtDbMedicine
    {
        [Column("Row#"), Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Row { get; set; }

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

        [Column("Strength_Unit_of_Measure"), StringLength(10)]
        public string StrengthUnitOfMeasure { get; set; }

        [Column("Dosage_Form"), StringLength(4)]
        public string DosageForm { get; set; }

        [Column("Brand_Name_Code"), StringLength(1)]
        public string BrandNameCode { get; set; }

        [Column("Package_Size")]
        public float? PackageSize { get; set; }

        [Column("Package_Size_Unit_of_Measure"), StringLength(2)]
        public string PackageSizeUnitOfMeasure { get; set; }

        [Column("Package_Quantity")]
        public int? PackageQuantity { get; set; }

        [Column("Unit_DoseUnit_of_Use_Package"), StringLength(1)]
        public string UnitDoseOfUsePackage { get; set; }

        [Column("Package_Description_Code"), StringLength(2)]
        public string PackageDescriptionCode { get; set; }

        [Column("Manufacturers_Labeler_Name"), StringLength(50)]
        public string Manufacturer { get; set; }

        [StringLength(1), Column("GPPC_Price_Code")]
        public string GppcPriceCode { get; set; }

        [Column("GPPC_Unit_Price")]
        public float? GppcUnitPrice { get; set; }

        [Column("Image_Filename"), StringLength(20)]
        public string MedicineImage { get; set; }

        [Column("Last_Change_Date")]
        public long? LastChangeDate { get; set; }

        public bool IsDuplicate { get; set; }
    }
}
