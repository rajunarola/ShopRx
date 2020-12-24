using RxFair.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Dto.Dtos;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.Extensions;
using RxFair.Dto.Enum;
using RxFair.Data.Utility;
using RxFair.Dto;

namespace RxFair.Service.Implemetation
{
    public class PharmacyRepository : GenericRepository<Pharmacy>, IPharmacyService
    {
        private readonly RxFairDbContext _context;
        public PharmacyRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<NewPharmacyDto>> GetNewPharmacyRequestList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetNewPharmacyRequestList, paraObjects);
            return Common.ConvertDataTable<NewPharmacyDto>(dataSet.Tables[0]);
        }

        public async Task<List<PharmacyAdvertisementDto>> GetPharmacyAdvertisementList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyAdvertisements, paraObjects);
            return Common.ConvertDataTable<PharmacyAdvertisementDto>(dataSet.Tables[0]);
        }

        public NewPharmacyDto GetPharmacyAdmin()
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetPharmacyAdmin);
            return Common.ConvertDataTable<NewPharmacyDto>(dataSet.Tables[0]).FirstOrDefault();
        }

        public async Task<List<UserBasicInfo>> GetPharmacyUserList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyUserList, paraObjects);
            return Common.ConvertDataTable<UserBasicInfo>(dataSet.Tables[0]);
        }

        public async Task<List<MedicineDto>> GetPharmacyMedicinesList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetPharmacyMedicines, paraObjects);
            return Common.ConvertDataTable<MedicineDto>(dataSet.Tables[0]);
        }
        public async Task<List<PharmacyOrderList>> GetPharmacyOrderList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.PharmacyOrderList, paraObjects);
            return Common.ConvertDataTable<PharmacyOrderList>(dataSet.Tables[0]);
        }

        public List<DropdownModel> GetOrderDistributorList()
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetOrderDistributorList);
            return Common.ConvertDataTable<DropdownModel>(dataSet.Tables[0]);
        }

        public List<PharmacyOrderDto> GetRewardCalculation()
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.RewardCalculation);
            return Common.ConvertDataTable<PharmacyOrderDto>(dataSet.Tables[0]);
        }
    }
}
