using RxFair.Data.DbContext;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;

namespace RxFair.Service.Interface
{
    public interface IUserService : IGenericService<ApplicationUser>
    {
        DashboardDto GetAdminDashboard();
        PharmacyDistributorDashboardDto GetPharmacyDistributorUserDashboard(SqlParameter[] paraObjects);
        Task<List<UserBasicInfo>> GetUserListAsync(SqlParameter[] paraObjects);
        Task<List<DistributorSubInfoDto>> GetDistributorSubInfoList();
        Task<List<RoleDropdownModel>> GetUserGroupWishRole(SqlParameter[] paraObjects);
    }

    public interface IUserAddressService : IGenericService<UserAddress>
    {
    }
}
