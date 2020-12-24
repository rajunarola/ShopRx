using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Implemetation
{
public  class AdvertisementRepository : GenericRepository<Advertisement>, IAdvertisementService
    {
        private readonly RxFairDbContext _context;
        public AdvertisementRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AdvertisementDto>> GetAdvertisementList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetAdvertisementList, paraObjects);
            return Common.ConvertDataTable<AdvertisementDto>(dataSet.Tables[0]);
        }

        public async Task<List<MedicineDto>> GetAdvertisementMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetAdvertisementMedicine, paraObjects);
            return Common.ConvertDataTable<MedicineDto>(dataSet.Tables[0]);
        }

        public async Task<List<AdvertisementDto>> GetAdvertisementRequestList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetAdvertisementRequestList, paraObjects);
            return Common.ConvertDataTable<AdvertisementDto>(dataSet.Tables[0]);
        }

        public async Task<List<MedicineDto>> GetMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMedicineList, paraObjects);
            return Common.ConvertDataTable<MedicineDto>(dataSet.Tables[0]);
        }
    }
}
