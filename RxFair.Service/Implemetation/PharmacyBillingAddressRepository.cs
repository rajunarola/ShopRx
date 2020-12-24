using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;

namespace RxFair.Service.Implemetation
{
    public class PharmacyBillingAddressRepository : GenericRepository<PharmacyBillingAddress>, IPharmacyBillingAddressService
    {
        private readonly RxFairDbContext _context;
        public PharmacyBillingAddressRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}