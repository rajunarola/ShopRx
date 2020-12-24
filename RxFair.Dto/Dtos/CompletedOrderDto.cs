using System.Collections.Generic;

namespace RxFair.Dto.Dtos
{
    public class CompletedOrderDto : BaseModel
    {
        public List<CompletedOrDistributorInfo> DistributorInfo { get; set; }
        public string OrderNumber { get; set; }
        public PharmacyBillOrShipAddressDto BillingAddress { get; set; }
        public PharmacyBillOrShipAddressDto ShippingAddress { get; set; }
    }

    public class CompletedOrDistributorInfo
    {
        public string ComapanyLogo { get; set; }
        public string DistributorCompanyName { get; set; }
        public decimal OrderTotal { get; set; }

    }

}
