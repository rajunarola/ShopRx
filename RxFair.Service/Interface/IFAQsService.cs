using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface IFaQsService : IGenericService<FAQs>
    {
        Task<List<FaQsView>> GetFaQsListAsync(SqlParameter[] paraObjects);
        Task<List<FaQsView>> GetFaQsListAsyncBySearch(SqlParameter[] paraObjects);
    }
}