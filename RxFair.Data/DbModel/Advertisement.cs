using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RxFair.Data.DbModel.BaseModel;

namespace RxFair.Data.DbModel
{
    public class Advertisement : EntityWithAudit
    {
        [Required]
        public short DealType { get; set; }

        public long DistributorId { get; set; }
        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DealDate { get; set; }
        public DateTime? PriceIncreaseDate { get; set; }

        public string Request { get; set; }
        public string Notes { get; set; }
        public bool? Status { get; set; }
        public DateTime? LastSentDate { get; set; }

        public virtual ICollection<AdvertisementMedicine> AdvertisementMedicines { get; set; }
    }
}
