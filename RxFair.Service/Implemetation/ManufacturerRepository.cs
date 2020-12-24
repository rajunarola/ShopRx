using System.Collections.Generic;
using System.Data.SqlClient;
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
    public class ManufacturerRepository : GenericRepository<ManufacturerMaster>, IManufacturerService
    {
        private readonly RxFairDbContext _context;
        public ManufacturerRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ManufacturerView>> GetManufacturerList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetManufacturerList, paraObjects);
            return Common.ConvertDataTable<ManufacturerView>(dataSet.Tables[0]);
        }
    }
}
