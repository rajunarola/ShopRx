using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
  public  class ProductPriceIncreaseDto
    {
        public string Request { get; set; }
        public DateTime PriceIncreateDate { get; set; }
        public string Note { get; set; }
        public List<MedicineDto> Medicine { get; set; }
        public int DistributorId { get; set; }
    }
}
