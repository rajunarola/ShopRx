using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class SystemModuleRepository : GenericRepository<SystemModule>, ISystemModuleService
    {
        private readonly RxFairDbContext _context;
        public SystemModuleRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public List<AllowedMenuViewModel> GetUserAllowedMenu(SqlParameter[] paraObjects)
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetUserAllowedMenuList, paraObjects);
            return Common.ConvertDataTable<AllowedMenuViewModel>(dataSet.Tables[0]);
        }

        public async Task<List<SystemModuleDto>> GetSystemModuleList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetSystemModuleList, paraObjects);
            return Common.ConvertDataTable<SystemModuleDto>(dataSet.Tables[0]);
        }
    }

    public class RolesModuleAccessRepository : GenericRepository<RolesModuleAccess>, IRolesModuleAccessService
    {
        private readonly RxFairDbContext _context;
        public RolesModuleAccessRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public List<RolesModuleAccessPermissionDto> GetRolesModuleAccess(SqlParameter[] paraObjects)
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetRolesModuleAccess, paraObjects);
            return Common.ConvertDataTable<RolesModuleAccessPermissionDto>(dataSet.Tables[0]);
        }

        public List<RolesModuleAccessDto> RolesModuleAccess(SqlParameter[] paraObjects)
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetRolesWishModuleAccess, paraObjects);
            return Common.ConvertDataTable<RolesModuleAccessDto>(dataSet.Tables[0]);
        }
    }

    //public class RolesModuleFunctionalityAccessRepository : GenericRepository<RolesModuleFunctionalityAccess>, IRolesModuleFunctionalityAccessService
    //{
    //    private readonly RxFairDbContext _context;
    //    public RolesModuleFunctionalityAccessRepository(RxFairDbContext context) : base(context)
    //    {
    //        _context = context;
    //    }
    //}

    public class AccessModuleFunctionalityRepository : GenericRepository<AccessModuleFunctionality>, IAccessModuleFunctionalityService
    {
        private readonly RxFairDbContext _context;
        public AccessModuleFunctionalityRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

    public class FunctionalityRepository : GenericRepository<Functionality>, IFunctionalityService
    {
        private readonly RxFairDbContext _context;
        public FunctionalityRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
