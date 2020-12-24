using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class StateRepository : GenericRepository<State>, IStateService
    {
        private readonly RxFairDbContext _context;
        public StateRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
