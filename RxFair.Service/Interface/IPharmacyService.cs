using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Dto;

namespace RxFair.Service.Interface
{
    public interface IPharmacyService : IGenericService<Pharmacy>
    {
        Task<List<NewPharmacyDto>>GetNewPharmacyRequestList(SqlParameter[] paraObjects);
        Task<List<UserBasicInfo>>GetPharmacyUserList(SqlParameter[] paraObjects);
        Task<List<PharmacyAdvertisementDto>>GetPharmacyAdvertisementList(SqlParameter[] paraObjects);
        Task<List<MedicineDto>>GetPharmacyMedicinesList(SqlParameter[] paraObjects);
        Task<List<PharmacyOrderList>>GetPharmacyOrderList(SqlParameter[] paraObjects);
        NewPharmacyDto GetPharmacyAdmin();
        List<DropdownModel> GetOrderDistributorList();
        List<PharmacyOrderDto> GetRewardCalculation();
        
    }
}