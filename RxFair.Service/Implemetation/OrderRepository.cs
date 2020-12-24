using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Data.Extensions;
using RxFair.Dto.Enum;
using RxFair.Dto.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using RxFair.Data.Utility;

namespace RxFair.Service.Implemetation
{
    public class OrderRepository : GenericRepository<Order>, IOrderService
    {
        private readonly RxFairDbContext _context;
        public OrderRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

    public class DistributorOrderRepository : GenericRepository<DistributorOrder>, IDistributorOrderService
    {
        private readonly RxFairDbContext _context;
        public DistributorOrderRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ViewOrderDetailDto>> GetOrdersDetails(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetOrdersDetails, paraObjects);
            return Common.ConvertDataTable<ViewOrderDetailDto>(dataSet.Tables[0]);
        }
    }

    public class DistributorOrderChargeRepository : GenericRepository<DistributorOrderCharge>, IDistributorOrderChargeService
    {
        private readonly RxFairDbContext _context;
        public DistributorOrderChargeRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<OrderInfoDto>> GetOrders(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetOrders, paraObjects);
            return Common.ConvertDataTable<OrderInfoDto>(dataSet.Tables[0]);
        }
        public async Task<List<PendingOrderDto>> GetPendingOrderList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPendingOrderList, paraObjects);
            return Common.ConvertDataTable<PendingOrderDto>(dataSet.Tables[0]);
        }
        public async Task<List<AllOrderDto>> GetAllOrderList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetAllOrderList, paraObjects);
            return Common.ConvertDataTable<AllOrderDto>(dataSet.Tables[0]);
        }

        public async Task<List<OrderchargeMedicineListDto>> GetDistributorOrderChargeMedList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorOrderChargeMedList, paraObjects);
            return Common.ConvertDataTable<OrderchargeMedicineListDto>(dataSet.Tables[0]);
        }
        public async Task<List<MedicinePurchaseHistoryDto>> GetMedicinePurchaseHistory(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMedicinePurchaseHistory, paraObjects);
            return Common.ConvertDataTable<MedicinePurchaseHistoryDto>(dataSet.Tables[0]);
        }

    }
}
