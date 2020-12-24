using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class BlogRepository : GenericRepository<Blog>, IBlogService
    {
        private readonly RxFairDbContext _context;
        public BlogRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<BlogDto>> GetBlogList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetBlogList, paraObjects);
            return Common.ConvertDataTable<BlogDto>(dataSet.Tables[0]);
        }
        public async Task<List<RecentlyBlogsView>> GetRecentBlogs()
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRecentBlogs);
            return Common.ConvertDataTable<RecentlyBlogsView>(dataSet.Tables[0]);
        }

    }
}