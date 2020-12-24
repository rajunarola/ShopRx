using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RxFair.Service.Interface
{
    public interface IOrderService : IGenericService<Order>
    {

    }

    public interface IDistributorOrderService : IGenericService<DistributorOrder>
    {
        Task<List<ViewOrderDetailDto>> GetOrdersDetails(SqlParameter[] paraObjects);
    }

    public interface IDistributorOrderChargeService : IGenericService<DistributorOrderCharge>
    {
        Task<List<OrderInfoDto>> GetOrders(SqlParameter[] paraObjects);
        Task<List<PendingOrderDto>> GetPendingOrderList(SqlParameter[] paraObjects);
        Task<List<AllOrderDto>> GetAllOrderList(SqlParameter[] paraObjects);
        Task<List<OrderchargeMedicineListDto>> GetDistributorOrderChargeMedList(SqlParameter[] paraObjects);
        Task<List<MedicinePurchaseHistoryDto>> GetMedicinePurchaseHistory(SqlParameter[] paraObjects);
    }
}