using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
    public class UploadedMedicineRepository : GenericRepository<UploadedMedicine>, IUploadedMedicineService
    {
        private readonly RxFairDbContext _context;
        public UploadedMedicineRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<UploadMedicine>> GetUploadedMedicineList(SqlParameter[] paraObjects)
        {
            var dataset = await _context.GetQueryDatatableAsync(StoredProcedureList.GetUploadedMedicineList, paraObjects.ToArray());
            return Common.ConvertDataTable<UploadMedicine>(dataset.Tables[0]);
        }
    }
}
