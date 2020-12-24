using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class DistributorSellMedicineDto:BaseModel
    {
        public bool InStock { get; set; }
        public long Stock { get; set; }
        public bool IsContracted { get; set; }
        public bool IsShortDated { get; set; }
        public float Price { get; set; }
        public string NDC { get; set; }
        public string UPC { get; set; }
        public string Strength { get; set; }
        public string ManufacturerName { get; set; }
        public string MedicineName { get; set; }
        public string Category { get; set; }
        public long MedicineId { get; set; }
    }
}
