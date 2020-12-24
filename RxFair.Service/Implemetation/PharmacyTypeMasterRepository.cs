using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;


namespace RxFair.Service.Implemetation
{
    public class PharmacyTypeMasterRepository : GenericRepository<PharmacyTypeMaster>, IPharmacyTypeMasterService
    {
        private readonly RxFairDbContext _context;
        public PharmacyTypeMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
