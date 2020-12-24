using System.Collections.Generic;

namespace RxFair.Dto.Dtos
{
    public class MedicineHistoryDto : BaseModel
    {
        public string Ndc { get; set; }
        public string Upc { get; set; }

        public string MedicineImage { get; set; }

        public string MedicineName { get; set; }

        public string DistributorName { get; set; }

        public float OldPrice { get; set; }

        public float NewPrice { get; set; }

        public string Createdby { get; set; }

        public string Createddate { get; set; }

        public List<MedicineHistoryDto> OldPriceList { get; set; }
    }
}
