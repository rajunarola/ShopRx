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
    public class CartRepository : GenericRepository<Cart>, ICartService
    {
        private readonly RxFairDbContext _context;
        public CartRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PlaceOrderDto>> GetCartList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetCartList, paraObjects);
            return Common.ConvertDataTable<PlaceOrderDto>(dataSet.Tables[0]);
        }

        public async Task<List<OrderSettingDto>> GetDistributorOrderSetting(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDistributorOrderSetting, paraObjects);
            return Common.ConvertDataTable<OrderSettingDto>(dataSet.Tables[0]);
        }
    }
}