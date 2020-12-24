using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Interface
{
  public  interface IAdvertisementService : IGenericService<Advertisement>
    {
        Task<List<AdvertisementDto>> GetAdvertisementList(SqlParameter[] paraObjects);
        Task<List<MedicineDto>> GetAdvertisementMedicineList(SqlParameter[] paraObjects);
        Task<List<AdvertisementDto>> GetAdvertisementRequestList(SqlParameter[] paraObjects);
        Task<List<MedicineDto>> GetMedicineList(SqlParameter[] paraObjects);
    }
}
