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
    public class FaQsRepository : GenericRepository<FAQs>, IFaQsService
    {
        private readonly RxFairDbContext _context;
        public FaQsRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<FaQsView>> GetFaQsListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetFaQsList, paraObjects);
            return Common.ConvertDataTable<FaQsView>(dataSet.Tables[0]);
        }
        public async Task<List<FaQsView>> GetFaQsListAsyncBySearch(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetFaQsListBySearch, paraObjects);
            return Common.ConvertDataTable<FaQsView>(dataSet.Tables[0]);
        }
    }
}
