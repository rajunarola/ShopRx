using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface INewDistributorRequestService : IGenericService<NewDistributorRequest>
    {
        Task<List<NewDistributorRequestDto>> GetNewDistributorRequestList(SqlParameter[] paraObjects);
        Task<int> AddDistributer(long id); 
        
    }
}
