using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class ReportDto : BaseModel
    {
        public long? OrderChargeId { get; set; }

        public string PharmacyName { get; set; }

        public string UniqueOrder { get; set; }

        public string Ndc { get; set; }
        public string Upc { get; set; }

        public string Category { get; set; }

        public string Strength { get; set; }
        public string Dosage { get; set; }
        public string Manufacturer { get; set; }

        public float PackageSize { get; set; }

        public long PackageQuantity { get; set; }

        public long Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal SubTotal { get; set; }

        public decimal GrandTotal { get; set; }

        public string MedicineName { get; set; }

        public string Distributor { get; set; }

        public float? DistributorPrice { get; set; }

        public bool DeliveryStatus { get; set; }

        public string OrderStatus { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public long MedicineId { get; set; }

        public List<string> DistributorList { get; set; }

        public List<ReportDto> PurchaseReport { get; set; }


    }
}
