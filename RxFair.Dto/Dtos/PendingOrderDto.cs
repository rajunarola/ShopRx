using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
  public  class PendingOrderDto:BaseModel
    {
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public string PharmacyName { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class AllOrderDto : PendingOrderDto
    {
        public string TrackingNo { get; set; }
        public string TrackingLink { get; set; }
        public short OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        
    }
}
