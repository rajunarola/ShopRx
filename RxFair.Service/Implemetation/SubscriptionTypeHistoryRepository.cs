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
    public class SubscriptionTypeHistoryRepository : GenericRepository<SubscriptionTypeHistory>, ISubscriptionTypeHistoryService
    {
        private readonly RxFairDbContext _context;
        public SubscriptionTypeHistoryRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SubscriptionTypeHistoryDto>> GetSubscriptionTypeHistoryList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetSubscriptionTypeHistoryList, paraObjects);
            return Common.ConvertDataTable<SubscriptionTypeHistoryDto>(dataSet.Tables[0]);
        }
    }
}