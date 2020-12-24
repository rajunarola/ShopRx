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
  public  interface ICartService : IGenericService<Cart>
    {
        Task<List<PlaceOrderDto>> GetCartList(SqlParameter[] paraObjects);
        Task<List<OrderSettingDto>> GetDistributorOrderSetting(SqlParameter[] paraObjects);
    }
}
