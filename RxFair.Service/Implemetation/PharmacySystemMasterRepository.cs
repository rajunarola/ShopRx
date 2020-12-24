using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;


namespace RxFair.Service.Implemetation
{
    public class PharmacySystemMasterRepository : GenericRepository<PharmacySystemMaster>, IPharmacySystemMasterService
    {
        private readonly RxFairDbContext _context;
        public PharmacySystemMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
