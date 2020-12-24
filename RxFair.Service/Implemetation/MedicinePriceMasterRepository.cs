using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class MedicinePriceMasterRepository : GenericRepository<MedicinePriceMaster>, IMedicinePriceMasterService
    {
        private readonly RxFairDbContext _context;
        public MedicinePriceMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}