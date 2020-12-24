using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;

namespace RxFair.Service.Implemetation
{
    public class BlogTagRepository : GenericRepository<BlogTag>, IBlogTagService
    {
        private readonly RxFairDbContext _context;
        public BlogTagRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
