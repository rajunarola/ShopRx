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
    public class DistributorSubscriptionRepository : GenericRepository<DistributorSubscription>, IDistributorSubscriptionService
    {
        private readonly RxFairDbContext _context;
        public DistributorSubscriptionRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DistributorSubscriptionDto>> GetDistributorSubscriptionList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorSubscriptionList, paraObjects);
            return Common.ConvertDataTable<DistributorSubscriptionDto>(dataSet.Tables[0]);
        }
    }
}