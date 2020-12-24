using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class DistributerOrderSettingRepository : GenericRepository<DistributorOrderSetting>, IDistributerOrderSettingService
    {
        private readonly RxFairDbContext _context;
        public DistributerOrderSettingRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

   
}