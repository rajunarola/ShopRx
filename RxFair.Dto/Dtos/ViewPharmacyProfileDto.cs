using System.Collections.Generic;

namespace RxFair.Dto.Dtos
{
    public class ViewPharmacyProfileDto
    {
        public NewPharmacyDto Pharmacy { get; set; }
        public List<PharmacyBillOrShipAddressDto> PharmacyBillAddresses { get; set; }
        public List<PharmacyBillOrShipAddressDto> PharmacyShipAddresses { get; set; }
    }

    public class PharmacyBillOrShipAddressDto : BaseModel
    {
        public long PharmacyId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public long StateId { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }
        public bool IsBilling { get; set; }
        public bool IsDefault { get; set; }
    }

    public class OrderPharmacyAddressListDto: ViewPharmacyProfileDto
    {
        public string PharmacyName { get; set; }
    }

}
