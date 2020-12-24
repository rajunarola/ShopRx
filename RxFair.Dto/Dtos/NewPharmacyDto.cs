using System;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class NewPharmacyDto : BaseModel
    {
        public string PharmacyName { get; set; }
        public long PharmacyTypeId { get; set; }
        public string PharmacyTypeName { get; set; }
        public long PharmacySystemId { get; set; }
        public string PharmacySystemName { get; set; }
        public string JobTitle { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryEmail { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string DeaNumber { get; set; }
        public string NpiNumber { get; set; }
        public string ReferCode { get; set; }
        public IFormFile LicenseFile { get; set; }
        public string FileLicense { get; set; }
        public DateTime LicenseExpires { get; set; }
        public string LicenseExpiresDate { get; set; }
        public IFormFile DeaFile { get; set; }
        public string FileDea { get; set; }
        public DateTime DeaExpries { get; set; }
        public string DeaExpriesDate { get; set; }
        public int? Status { get; set; }
        public long? UserId { get; set; }

        //Billing information for Pharmacy
        public string BillAddress1 { get; set; }
        public string BillAddress2 { get; set; }
        public string BillCity { get; set; }
        public long BillState { get; set; }
        public string BillStatName { get; set; }
        public string BillZipCode { get; set; }

        //Shipping information for Pharmacy
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryCity { get; set; }
        public long DeliveryState { get; set; }
        public string DeliveryStatName { get; set; }
        public string DeliveryZipCode { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
