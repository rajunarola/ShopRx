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
    public interface IReportService : IGenericService<DistributorOrder>
    {
        Task<List<ReportDto>> GetPharmacyPurchaseOrder(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetDistributorSalesOrder(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetReviewMedicinePrice(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetReviewMedicineDistributorWise(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetPharmacyOrderSummary(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetUnshippedOrders(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetSalesOrderReport(SqlParameter[] paraObjects);
        Task<List<ReportDto>> GetPharmacyPurchaseDistributorOrder(SqlParameter[] paraObjects);
    }
}
