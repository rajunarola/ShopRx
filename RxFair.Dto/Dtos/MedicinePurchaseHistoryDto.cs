using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
   public class MedicinePurchaseHistoryDto:BaseModel
    {
        public string OrderDate { get; set; }
        public string OrderId { get; set; }
        public float? PackageSize { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string CompanyName { get; set; }
        public string NDC { get; set; }
    }
}
