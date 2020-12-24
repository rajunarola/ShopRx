using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{

    public class TermsAndConditonRepository : GenericRepository<TermsAndCondition>, ITermsAndConditionService
    {
        private readonly RxFairDbContext _context;
        public TermsAndConditonRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
