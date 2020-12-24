using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class WatchListDto : BaseModel
    {
        public bool IsExist { get; set; }

        public bool tempFlag { get; set; }

        public float MatchPrice { get; set; }

        public int Quantity { get; set; }

        public long? MedicineId { get; set; }

        public DateTime? MedicineDate { get; set; }

        public string MedicineName { get; set; }

        public string Manufacturer { get; set; }

        public string Dosage { get; set; }

        public float PackageSize { get; set; }

        public string Category { get; set; }

        public string Ndc { get; set; }

        public string Strength { get; set; }

        public float? Price { get; set; }

        public long DistributorId { get; set; }

        public string DistributorName { get; set; }
    }
}
