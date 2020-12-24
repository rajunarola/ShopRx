using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Implemetation
{
    public class CommissionHistoryRepository : GenericRepository<CommissionHistory>, ICommissionHistoryService
    {
        private readonly RxFairDbContext _context;
        public CommissionHistoryRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CommissionInvoiceDto>> GetDistributorCommissionList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorCommissionList, paraObjects);
            return Common.ConvertDataTable<CommissionInvoiceDto>(dataSet.Tables[0]);
        }

        public async Task<List<GetCommissionDto>> GetCommissionInvoicePaymentList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetCommissionInvoicePaymentList, paraObjects);
            return Common.ConvertDataTable<GetCommissionDto>(dataSet.Tables[0]);
        }
        public async Task<List<GetCommissionDto>> GetPendingPaymentInvoiceIdList()
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPendingCommssionIdlist);
            return Common.ConvertDataTable<GetCommissionDto>(dataSet.Tables[0]);
        }
        public async Task<PendingPaymentDto> GetPendingPaymentlist(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPendingPaymentlist, paraObjects);
            return Common.ConvertDataTable<PendingPaymentDto>(dataSet.Tables[0]).FirstOrDefault();
        }

        public async Task<List<DistributorCommissioninvoiceDto>> GetDistributorCommissionInvoiceList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorCommissionInvoiceList, paraObjects);
            return Common.ConvertDataTable<DistributorCommissioninvoiceDto>(dataSet.Tables[0]);
        }

        public async Task<List<PendingCommissionCalculationDto>> GetPendingCommissionCalculation()
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPendingCommissionCalculation);
            return Common.ConvertDataTable<PendingCommissionCalculationDto>(dataSet.Tables[0]);
        }

    }

    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceService
    {
        private readonly RxFairDbContext _context;
        public InvoiceRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

    public class InvoicePaymentRepository : GenericRepository<InvoicePayment>, IInvoicePaymentService
    {
        private readonly RxFairDbContext _context;
        public InvoicePaymentRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
