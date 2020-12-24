using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class SystemMedicineDto : BaseModel
    {
        public string NDC { get; set; }
        public string UPC { get; set; }
        public string MedicineImage { get; set; }
        public string MedicineName { get; set; }
        public string Strength { get; set; }
        public float PackageSize { get; set; }
        public string ManufacturerName { get; set; }
        public string Category { get; set; }
        public string CategoryType { get; set; }

        public long? DistributorId { get; set; }

    }
    public class SearchMedicineDto : SystemMedicineDto
    {
        public string DistributorName { get; set; }
        public float AwpPrice { get; set; }
        public float WacPrice { get; set; }
        public bool InStock { get; set; }
        public string Stock { get; set; }
        public bool IsContracted { get; set; }
        public bool IsShortDated { get; set; }
        public bool IsBestDeal { get; set; }
        public bool IsCheap { get; set; }
        public string Description { get; set; }
        public string DosageForm { get; set; }
        public string IsPrevioslyPurchased { get; set; }
        public long UniqueId { get; set; }
     
    }
    public class PlaceOrderDto : SearchMedicineDto
    {
        public int Quantity { get; set; }
        public long MedicineId { get; set; }
    }

    public class ViewPlaceOrderDto
    {
        public List<IGrouping<long?, PlaceOrderDto>> CartList { get; set; }
        public List<OrderSettingDto> DistributorOrderSettings { get; set; }
    }

    public class OrderSettingDto : BaseModel
    {
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public int TimezoneId { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal OverNightAmount { get; set; }
        public bool ServiceDay { get; set; }
        public string _CutOffTime { get; set; }
        public TimeSpan? CutOffTime { get; set; }
        public long DistributorId { get; set; }

    }


}
