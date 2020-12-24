using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class OrderAddressListDto : BaseModel 
   {
    public List<PharmacyBillingAddressDto> billingAddress { get; set; }
    public List<PharmacyshippingAddressDto> shippingAddress { get; set; }
   } 

   public class PharmacyBillingAddressDto: BaseModel
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public long StateId { get; set; }

        public string ZipCode { get; set; }

        public bool IsDefault { get; set; }
    }
    public class PharmacyshippingAddressDto : PharmacyBillingAddressDto
    {
        
    }

    public class PharmacyOrderDto 
    {
        public long PharmacyId { get; set; }
        public long OrderId { get; set; }
        public decimal SubTotal { get; set; }
        public int LastOrderDays { get; set; }
    }
    
}
