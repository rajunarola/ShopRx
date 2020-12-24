using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
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
    public class AdvertiseTemplateRepository: GenericRepository<AdvertiseEmailTemplate>, IAdvertiseTemplateService
    {
        private readonly RxFairDbContext _context;
        public AdvertiseTemplateRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AdvertiseTemplateDto>> GetAdvertiseTemplateList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetAdvertiseList, paraObjects);
            return Common.ConvertDataTable<AdvertiseTemplateDto>(dataSet.Tables[0]);
        }

    }
   
    
}
