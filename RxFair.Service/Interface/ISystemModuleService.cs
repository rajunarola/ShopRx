using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface ISystemModuleService : IGenericService<SystemModule>
    {
        List<AllowedMenuViewModel> GetUserAllowedMenu(SqlParameter[] paraObjects);
        Task<List<SystemModuleDto>> GetSystemModuleList(SqlParameter[] paraObjects);
    }

    public interface IRolesModuleAccessService : IGenericService<RolesModuleAccess>
    {
        List<RolesModuleAccessPermissionDto> GetRolesModuleAccess(SqlParameter[] paraObjects);
        List<RolesModuleAccessDto> RolesModuleAccess(SqlParameter[] paraObjects);
    }

    //public interface IRolesModuleFunctionalityAccessService : IGenericService<RolesModuleFunctionalityAccess>
    //{
    //}

    public interface IAccessModuleFunctionalityService : IGenericService<AccessModuleFunctionality>
    {
    }

    public interface IFunctionalityService : IGenericService<Functionality>
    {
    }
}
