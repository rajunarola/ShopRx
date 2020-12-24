using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
    public class UploadMedicineDto
    {
        public  IFormFile file { get; set; }
        public long DistributorId { get; set; }

    }
}
