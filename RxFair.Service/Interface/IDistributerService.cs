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
    public interface IDistributorService : IGenericService<Distributor>
    {
        Task<List<DistributorDto>> GetDistributorList(SqlParameter[] paraObjects);
        Task<List<UserBasicInfo>> GetDistributorUserList(SqlParameter[] paraObjects);
        List<DropdownModel> GetDistributorAdminList();
        
    }
}
