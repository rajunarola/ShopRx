using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
public    class DistributorMedicineDto:BaseModel
    {
        public long DistributorId { get; set; }
        public long MedicineId { get; set; }
    }
}
