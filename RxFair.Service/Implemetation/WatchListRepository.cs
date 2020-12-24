using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    public class WatchListRepository : GenericRepository<WatchList>, IWatchlistService
    {
        private readonly RxFairDbContext _context;
        public WatchListRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<WatchListDto> GetWatchListDetails(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetWatchListDetails, paraObjects);
            return Common.ConvertDataTable<WatchListDto>(dataSet.Tables[0]).FirstOrDefault();
        }
        public async Task<List<WatchListDto>> GetWatchList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetWatchList, paraObjects);
            return Common.ConvertDataTable<WatchListDto>(dataSet.Tables[0]);
        }
        public async Task<List<WatchListDto>> GetWatchListOnSearch(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetWatchListOnSearch, paraObjects);
            return Common.ConvertDataTable<WatchListDto>(dataSet.Tables[0]);
        }
    }
}
