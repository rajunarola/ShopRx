using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;

namespace RxFair.Service.Implemetation
{
    public class BlogImageRepository : GenericRepository<BlogImage>, IBlogImageService
    {
        private readonly RxFairDbContext _context;
        public BlogImageRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
