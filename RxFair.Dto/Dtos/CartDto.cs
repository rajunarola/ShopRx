using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
  public  class CartDto:BaseModel
    {
        public int Quantity { get; set; }
        public long  MedicineId { get; set; }
        public long  DistributorId { get; set; }
        public long PharmacyId { get; set; }
        public DateTime  CartDate { get; set; }
    }
}
