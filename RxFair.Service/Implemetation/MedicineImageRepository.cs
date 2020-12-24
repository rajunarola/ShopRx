using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class MedicineImageRepository : GenericRepository<MedicineImage>, IMedicineImageService
    {
        private readonly RxFairDbContext _context;
        public MedicineImageRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}