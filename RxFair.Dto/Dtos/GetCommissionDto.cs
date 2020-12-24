using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
   public class GetCommissionDto:BaseModel
    {
        public long InvoiceId { get; set; }
        public string CompanyName { get; set; }
        public string InvoiceDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string PaidDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string paidby { get; set; }
    }

    public class PendingPaymentDto : GetCommissionDto
    {

        public decimal PendingAmount { get; set; }
    }

    public class DistributorCommissioninvoiceDto : GetCommissionDto
    {
        public short PaymentStatus { get; set; }
        public string PaymentDate { get; set; }
    }

    public class PendingCommissionCalculationDto
    {
        public long Id { get; set; }
        public decimal CommissionAmount { get; set; }
        public long DistributorId { get; set; }
    }

}
