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
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Implemetation
{
    public class ReportRepository:GenericRepository<DistributorOrder>,IReportService
    {
        private readonly RxFairDbContext _context;
        public ReportRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

       
        public async Task<List<ReportDto>> GetPharmacyPurchaseOrder(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyPurchaseOrder, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }
        public async Task<List<ReportDto>> GetPharmacyPurchaseDistributorOrder(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyPurchaseDistributorOrder, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }

        public async Task<List<ReportDto>> GetDistributorSalesOrder(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorSalesOrder, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }
        public async Task<List<ReportDto>> GetReviewMedicinePrice(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetReviewMedicinePrice, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }
        public async Task<List<ReportDto>> GetReviewMedicineDistributorWise(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetReviewMedicineDistributorWise, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }
        public async Task<List<ReportDto>> GetPharmacyOrderSummary(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyOrderSummary, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }

        public async Task<List<ReportDto>> GetUnshippedOrders(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetUnshippedOrders, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }

        public async Task<List<ReportDto>> GetSalesOrderReport(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetSalesOrderReport, paraObjects);
            return Common.ConvertDataTable<ReportDto>(dataSet.Tables[0]);
        }
    }
}
