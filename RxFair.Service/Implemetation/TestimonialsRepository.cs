using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;

namespace RxFair.Service.Implemetation
{
    public class TestimonialsRepository : GenericRepository<Testimonials>, ITestimonialsService
    {
        private readonly RxFairDbContext _context;
        public TestimonialsRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TestimonialDto>> GetTestimonialsListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetTestimonialsList, paraObjects);
            return Common.ConvertDataTable<TestimonialDto>(dataSet.Tables[0]);

        }
    }
}
