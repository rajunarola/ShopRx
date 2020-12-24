using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class DealOfTheDayDto
    {
        public string Request { get; set; }
        public DateTime DealDate { get; set; }
        public string Note { get; set; }
        public List<MedicineDto> Medicine { get; set; }
        public int DistributorId { get; set; }
    }
}
