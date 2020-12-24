using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RxFair.Service.Implemetation
{
    public class DistributorRepository : GenericRepository<Distributor>, IDistributorService
    {
        
        private readonly RxFairDbContext _context;
        public DistributorRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DistributorDto>> GetDistributorList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorList, paraObjects);
            return Common.ConvertDataTable<DistributorDto>(dataSet.Tables[0]);
        }

        public async Task<List<UserBasicInfo>> GetDistributorUserList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorUserList, paraObjects);
            return Common.ConvertDataTable<UserBasicInfo>(dataSet.Tables[0]);
        }

        public List<DropdownModel> GetDistributorAdminList()
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetDistributorAdminList);
            return Common.ConvertDataTable<DropdownModel>(dataSet.Tables[0]);
        }
    }
}