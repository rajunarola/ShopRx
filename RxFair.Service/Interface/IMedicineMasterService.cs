using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RxFair.Service.Interface
{
    public interface IMedicineMasterService : IGenericService<MedicineMaster>
    {
        Task<List<UploadMedicine>> GetDuplicateMedicineList();
        Task<List<SystemMedicineDto>> GetSystemMedicineList(SqlParameter[] paraObjects);
        Task<List<SystemMedicineDto>> GetDistributorSystemMedicineList(SqlParameter[] paraObjects);
        Task<List<DistributorSellMedicineDto>> GetDistributorMySellMedicineList(SqlParameter[] paraObjects);
        Task<ViewMedicineDto> GetViewMedicineInfo(SqlParameter[] paraObjects);
        Task<List<MedicineHistoryDto>> GetMedicineHistory(SqlParameter[] paraObjects);
        Task<List<MedicineHistoryDto>> GetMedicineHistoryById(SqlParameter[] paraObjects);


        #region  Order
        Task<List<SearchMedicineDto>> SearchMedicineList(SqlParameter[] paraObjects);
        Task<List<SearchMedicineDto>> MedicineSearch(SqlParameter[] paraObjects);
        Task<List<DropdownModel>> GetMedicineNameList(SqlParameter[] paraObjects);
        
        #endregion            
    }

    public interface IMedicineImageService : IGenericService<MedicineImage>
    {

    }
}