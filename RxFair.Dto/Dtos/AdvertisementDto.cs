using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class AdvertisementDto : BaseModel
    {
        public short DealType { get; set; }

        public DateTime? StartDate { get; set; }
        public string AdvStartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public string AdvEndDate { get; set; }

        public DateTime? DealDate { get; set; }
        public string AdvDealDate { get; set; }

        public DateTime? PriceIncreaseDate { get; set; }
        public string AdvPriceIncreaseDate { get; set; }

        public string Request { get; set; }

        public long DistributorId { get; set; }

        public bool? Status { get; set; }

        public string Notes { get; set; }

        public List<MedicineDto> Medicine { get; set; }

    }

    public class MedicineDto : BaseModel
    {
        public string NDC { get; set; }
        public string UPC { get; set; }

        public string MedicineName { get; set; }

        public string Strength { get; set; }

        public string Dosage { get; set; }

        public float? PackageSize { get; set; }

        public float Price { get; set; }

        public float DealPrice { get; set; }

        public string ManufacturerName { get; set; }

        public DateTime? EndDate { get; set; }

        public long? DistributorId { get; set; }

        public DateTime? AdvertisementDate { get; set; }
        public string DateOfAdvertisement { get; set; }

        public int Quantity { get; set; }
    }
}
