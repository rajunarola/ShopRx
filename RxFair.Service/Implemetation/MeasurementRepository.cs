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
    public class MeasurementRepository : GenericRepository<Measurement>, IMeasurementService
    {
        private readonly RxFairDbContext _context;
        public MeasurementRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<MeasurementView>> GetMeasurementListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMeasurementList, paraObjects);
            return Common.ConvertDataTable<MeasurementView>(dataSet.Tables[0]);
        }
    }
}
