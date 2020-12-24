using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class ImportMedicineData
    {
        public IList<UploadMedicine> UploadMedicines { get; set; }
        public UploadMedicine SingleMedicines { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UploadMedicinePriceData
    {
        public IFormFile File { get; set; }
        public long DistributorId { get; set; }
    }

    public class ImportMedicinePriceData
    {
        public IList<UploadMedicinePrice> UploadMedicinePrices { get; set; }
        public string ErrorMessage { get; set; }
    }


    public class UploadMedicine : BaseModel
    {
        public string Ndc { get; set; }
        public string Upc { get; set; }
        public string MedicineName { get; set; }
        public string MedicineImage { get; set; }
        public string Strength { get; set; }
        public long? StrengthId { get; set; }
        public string DosageForm { get; set; }
        public long? DosageFormId { get; set; }
        public long? DistributorId { get; set; }
        public string Distributor { get; set; }
        public string Brand { get; set; }
        public long? BrandId { get; set; }
        public string PackagingSize { get; set; }
        public float? PackageSize { get; set; }
        public long? PackagingSizeId { get; set; }
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string Manufacturer { get; set; }
        public long? ManufacturerId { get; set; }
        public string PackageDescriptionCode { get; set; }
        public long? PackageDescriptionCodeId { get; set; }
        public string Category { get; set; }
        public long? CategoryId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Flavour { get; set; }
        public int  TotalRecords1 { get; set; }
        public bool IsNdc { get; set; }

    }

    public class AddEditMedicine : UploadMedicine
    {
        public float? AwpPrice { get; set; }
        public bool? IsApprove { get; set; }
        public IFormFile MedicineFile { get; set; }
    }

    public class UploadMedicinePrice : BaseModel
    {
        public string Ndc { get; set; }
        public float? Price { get; set; }
        public bool ShortDated { get; set; }
        public bool Contracted { get; set; }
        public bool InStock { get; set; }
        public long? Stock { get; set; }
    }

    public class ViewMedicineDto : BaseModel
    {
        public string Ndc { get; set; }
        public string Upc { get; set; }
        public string MedicineName { get; set; }
        public string MedicineImage { get; set; }
        public string Strength { get; set; }
        public string StrengthCode { get; set; }
        public string DosageForm { get; set; }
        public string Distributor { get; set; }
        public string Brand { get; set; }
        public string PackageSizeCode { get; set; }
        public float? PackageSize { get; set; }
        public string PackageDecCode { get; set; }
        public string UnitSize { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
    }

    public class OrderchargeMedicineListDto: ViewMedicineDto{

        public int Quantity { get; set; }
        public long MedicineId { get; set; }
        public decimal Price { get; set; }
    }

    public class MedicinePurchaseHistoryViewDto : ViewMedicineDto
    {
        public string  Flavour { get; set; }
    } 
}
