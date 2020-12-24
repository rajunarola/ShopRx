using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Interface
{
   public  interface ICommissionHistoryService: IGenericService<CommissionHistory>
    {
        Task<List<CommissionInvoiceDto>> GetDistributorCommissionList(SqlParameter[] paraObjects);
        Task<List<GetCommissionDto>> GetCommissionInvoicePaymentList(SqlParameter[] paraObjects);
        Task<List<GetCommissionDto>> GetPendingPaymentInvoiceIdList();
        Task<PendingPaymentDto> GetPendingPaymentlist(SqlParameter[] paraObjects);
        Task<List<DistributorCommissioninvoiceDto>> GetDistributorCommissionInvoiceList(SqlParameter[] paraObjects);
        Task<List<PendingCommissionCalculationDto>> GetPendingCommissionCalculation();
        
    }

    public interface IInvoiceService : IGenericService<Invoice>
    { 

    }
    public interface IInvoicePaymentService : IGenericService<InvoicePayment>
    {

    }
}
