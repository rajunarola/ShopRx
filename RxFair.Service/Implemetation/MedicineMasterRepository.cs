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
    public class MedicineMasterRepository : GenericRepository<MedicineMaster>, IMedicineMasterService
    {
        private readonly RxFairDbContext _context;
        public MedicineMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SystemMedicineDto>> GetDistributorSystemMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorSystemMedicineList, paraObjects);
            return Common.ConvertDataTable<SystemMedicineDto>(dataSet.Tables[0]);
        }

        public async Task<List<DistributorSellMedicineDto>> GetDistributorMySellMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorMySellMedicineList, paraObjects);
            return Common.ConvertDataTable<DistributorSellMedicineDto>(dataSet.Tables[0]);
        }

        public async Task<List<UploadMedicine>> GetDuplicateMedicineList()
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDuplicateMedicineList, new SqlParameter[] { });
            return Common.ConvertDataTable<UploadMedicine>(dataSet.Tables[0]);
        }

        public async Task<List<SystemMedicineDto>> GetSystemMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetSystemMedicineList, paraObjects);
            return Common.ConvertDataTable<SystemMedicineDto>(dataSet.Tables[0]);
        }

        public async Task<ViewMedicineDto> GetViewMedicineInfo(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.ViewMedicineInfo, paraObjects);
            return Common.ConvertDataTable<ViewMedicineDto>(dataSet.Tables[0]).FirstOrDefault();
        }

        public async Task<List<SearchMedicineDto>> SearchMedicineList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.SearchMedicineList, paraObjects);
            return Common.ConvertDataTable<SearchMedicineDto>(dataSet.Tables[0]);
        }
        public async Task<List<SearchMedicineDto>> MedicineSearch(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.MedicineSearch, paraObjects);
            return Common.ConvertDataTable<SearchMedicineDto>(dataSet.Tables[0]);
        }


        public async Task<List<DropdownModel>> GetMedicineNameList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMedicineNameList, paraObjects);
            return Common.ConvertDataTable<DropdownModel>(dataSet.Tables[0]);
        }
        public async Task<List<MedicineHistoryDto>> GetMedicineHistory(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMedicineHistory, paraObjects);
            return Common.ConvertDataTable<MedicineHistoryDto>(dataSet.Tables[0]);
        }

        public async Task<List<MedicineHistoryDto>> GetMedicineHistoryById(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMedicineHistoryById, paraObjects);
            return Common.ConvertDataTable<MedicineHistoryDto>(dataSet.Tables[0]);
        }
    }
}
