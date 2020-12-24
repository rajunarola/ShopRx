using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    [Table("MedicinePriceHistory")]
    public class MedicinePriceHistory
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long MedicinePriceId { get; set; }
        [ForeignKey("MedicinePriceId")]
        public virtual MedicinePriceMaster MedicinePriceMaster { get; set; }

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

        public bool IsShortDated { get; set; }

        public bool IsContracted { get; set; }

        public bool InStock { get; set; }
        
        public long? Stock { get; set; }

        public DateTime CreatedDate { get; set; }

        public long ModifiedBy { get; set; }
    }
}