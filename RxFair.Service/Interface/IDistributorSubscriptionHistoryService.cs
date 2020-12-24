using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface IDistributorSubscriptionHistoryService : IGenericService<DistributorSubscriptionHistory>
    {
        Task<List<DistributorSubscriptionHistoryDto>> GetDistributorSubscriptionHistoryList(SqlParameter[] paraObjects);
    }
}
