using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class MedicinePriceMaster : EntityWithAudit
    {
        public MedicinePriceMaster()
        {
            IsShortDated = false;
            IsContracted = false;
            InStock = false;
        }
        public long MedicineId { get; set; }
        [ForeignKey("MedicineId")]
        public virtual MedicineMaster MedicineMaster { get; set; }

        public long? DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor DistributorMedicine { get; set; }

        [Required, Column("NDC_UPC_HRI"), StringLength(11)]
        public string NdcUpcHri { get; set; }

        [StringLength(8), Column("Generic_Product_Packaging_Code")]
        public string GenericProductPackagingCode { get; set; }

        [StringLength(1), Column("GPPC_Price_Code")]
        public string GppcPriceCode { get; set; }

        [Column("GPPC_Unit_Price")]
        public float? GppcUnitPrice { get; set; }

        [Column("AWPUnit_Price")]
        public float? AwpunitPrice { get; set; }

        [Column("AWPUnit_Price_Extended")]
        public float? AwpunitPriceExtended { get; set; }

        [Column("AWPPackage_Price")]
        public float? AwppackagePrice { get; set; }

        [Column("WACUnit_Price")]
        public float? WacunitPrice { get; set; }

        [Column("WACUnit_Price_Extended")]
        public float? WacunitPriceExtended { get; set; }

        [Column("WACPackage_Price")]
        public float? WacpackagePrice { get; set; }

        [Column("Effective_Date")]
        public long? EffectiveDate { get; set; }

        public bool IsShortDated { get; set; }

        public bool IsContracted { get; set; }
        
        public bool InStock { get; set; }

        public long? Stock { get; set; }

    }
}
