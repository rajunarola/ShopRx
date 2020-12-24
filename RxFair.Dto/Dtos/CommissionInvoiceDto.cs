using System;
using System.Collections.Generic;
using System.Text;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Dto.Dtos
{
    public class CommissionInvoiceDto:BaseModel
    {
        public string CompanyName { get; set; }
        public string InvoiceDate { get; set; }
        public Decimal? Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public long DistributorId { get; set; }
    }
}
