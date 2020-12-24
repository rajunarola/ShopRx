using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface ISubscriptionTypeService : IGenericService<SubscriptionType>
    {
        Task<List<SubscriptionTypeDto>> GetSubscriptionTypeListAsync(SqlParameter[] paraObjects);
    }
}
