using System;
using System.Linq;
using Microsoft.IdentityModel.Protocols;
using Remotion.Linq.Clauses;
using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Dto.Dtos;
using RxFair.Service.Implemetation.BaseService;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Data.Utility;
using RxFair.Data.Extensions;
using RxFair.Dto.Enum;

namespace RxFair.Service.Implemetation
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserService
    {
        private readonly RxFairDbContext _context;
        public UserRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public DashboardDto GetAdminDashboard()
        {
            var model = new DashboardDto();
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetAdminDashboard);
            model.UserDto = Common.ConvertDataTable<DashboardUserDto>(dataSet.Tables[0]).FirstOrDefault();
            model.OrderDto = Common.ConvertDataTable<DashboardOrderDto>(dataSet.Tables[1]).FirstOrDefault();
            return model;
        }

        public PharmacyDistributorDashboardDto GetPharmacyDistributorUserDashboard(SqlParameter[] paraObjects)
        {
            var model = new PharmacyDistributorDashboardDto();
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetPharmacyDistributorDashboard, paraObjects);
            model.UserDto = Common.ConvertDataTable<PharmacyDistributorUserDashboardDto>(dataSet.Tables[0]).FirstOrDefault();
            model.OrderDto = Common.ConvertDataTable<DashboardOrderDto>(dataSet.Tables[1]).FirstOrDefault();
            //model.OrderDto = new DashboardOrderDto
            //{
            //    Cancelled = 0,
            //    Confirmed = 0,
            //    Delivered = 0,
            //    PartialReturned = 0,
            //    Pending = 0,
            //    Returned = 0,
            //    Shipped = 0,
            //    TotalOrder = 0
            //};
            return model;
        }

        public async Task<List<UserBasicInfo>> GetUserListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetUserList, paraObjects);
            return Common.ConvertDataTable<UserBasicInfo>(dataSet.Tables[0]);
        }
        public async Task<List<DistributorSubInfoDto>> GetDistributorSubInfoList()
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorSubInfoList);
            return Common.ConvertDataTable<DistributorSubInfoDto>(dataSet.Tables[0]);
        }

        public async Task<List<RoleDropdownModel>> GetUserGroupWishRole(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetUserGroupWishRole, paraObjects);
            return Common.ConvertDataTable<RoleDropdownModel>(dataSet.Tables[0]);
        }
    }

    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressService
    {
        private readonly RxFairDbContext _context;
        public UserAddressRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

    }
}




