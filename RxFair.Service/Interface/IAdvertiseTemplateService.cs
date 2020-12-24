using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface IAdvertiseTemplateService : IGenericService<AdvertiseEmailTemplate>
    {
        Task<List<AdvertiseTemplateDto>> GetAdvertiseTemplateList(SqlParameter[] paraObjects);
    }
}
