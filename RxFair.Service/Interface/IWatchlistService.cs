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
    public interface IWatchlistService : IGenericService<WatchList>
    {
        Task<WatchListDto> GetWatchListDetails(SqlParameter[] paraObjects);
        Task<List<WatchListDto>> GetWatchList(SqlParameter[] paraObjects);
        Task<List<WatchListDto>> GetWatchListOnSearch(SqlParameter[] paraObjects);

    }
}
