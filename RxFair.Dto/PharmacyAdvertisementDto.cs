using System;
using System.Collections.Generic;
using System.Text;
using RxFair.Dto.Dtos;

namespace RxFair.Dto
{
    public class PharmacyAdvertisementDto:BaseModel
    {
        public string DistributorName { get; set; }

        public int NumberOfAdvertisements { get; set; }
    }
}
