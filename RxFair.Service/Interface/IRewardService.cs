using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{
    public interface IRewardTypeMasterService : IGenericService<RewardTypeMaster>
    {
    }

    public interface IRewardMoneyMasterService : IGenericService<RewardMoneyMaster>
    {
        List<RewardMoneyDto> GetRewardMoneyList();
        Task<List<RewardMoneyDto>> GetMoneyRangeList(SqlParameter[] paraObjects);
    }
    public interface IRewardEarnService : IGenericService<RewardEarn>
    {
        Task<List<RewardEarnDto>> GetRewardEarnHistoryList(SqlParameter[] paraObjects);
        Task<List<RewardEarnDto>> GetEarnMoneyList(SqlParameter[] paraObjects);
        Task<RewardEarnProductDto> GetRewardEarnProducts(SqlParameter[] paraObjects);
        Task<List<EarnedRewardProducts>> GetEarnedRewardProductList(SqlParameter[] paraObjects);
        Task<RebateProgressDto> GetRebateProgressInfo(SqlParameter[] paraObjects);
    }

    public interface IRewardProductService : IGenericService<RewardProduct>
    {
        Task<List<RewardProductDto>> GetRewardProductListAsync(SqlParameter[] paraObjects);
    }

    public interface IRedeemRequestService : IGenericService<RedeemRequest>
    {
        Task<List<RedeemRequestDto>> GetRedeemRequestList(SqlParameter[] paraObjects);
    }

    public interface IRewardMonthDaysService : IGenericService<RewardMonthDays>
    {
        
    }

}
