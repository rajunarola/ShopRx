using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using RxFair.Service.Interface;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;

namespace RxFair.Service.Implemetation
{
    public class RewardTypeMasterRepository : GenericRepository<RewardTypeMaster>, IRewardTypeMasterService
    {
        private readonly RxFairDbContext _context;
        public RewardTypeMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
    public class RewardMoneyMasterRepository : GenericRepository<RewardMoneyMaster>, IRewardMoneyMasterService
    {
        private readonly RxFairDbContext _context;
        public RewardMoneyMasterRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public List<RewardMoneyDto> GetRewardMoneyList()
        {
            var dataSet = _context.GetQueryDatatable(StoredProcedureList.GetRewardMoneyList);
            return Common.ConvertDataTable<RewardMoneyDto>(dataSet.Tables[0]);
        }

        public async Task<List<RewardMoneyDto>> GetMoneyRangeList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetMoneyRangeList, paraObjects);
            return Common.ConvertDataTable<RewardMoneyDto>(dataSet.Tables[0]);
        }
    }
    public class RewardEarnRepository : GenericRepository<RewardEarn>, IRewardEarnService
    {
        private readonly RxFairDbContext _context;
        public RewardEarnRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RewardEarnDto>> GetRewardEarnHistoryList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRewardEarnHistoryList, paraObjects);
            return Common.ConvertDataTable<RewardEarnDto>(dataSet.Tables[0]);
        }

        public async Task<List<RewardEarnDto>> GetEarnMoneyList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetEarnMoneyList, paraObjects);
            return Common.ConvertDataTable<RewardEarnDto>(dataSet.Tables[0]);
        }

        public async Task<RewardEarnProductDto> GetRewardEarnProducts(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRewardEarnProductList, paraObjects);
            var model = new RewardEarnProductDto
            {
                AvailableReward = Common.ConvertDataTable<AvailableReward>(dataSet.Tables[0]).FirstOrDefault(),
                ProductDtos = Common.ConvertDataTable<RewardProductDto>(dataSet.Tables[1]),
            };
            return model;
        }

        public async Task<List<EarnedRewardProducts>> GetEarnedRewardProductList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetEarnedRewardProductList, paraObjects);
            return Common.ConvertDataTable<EarnedRewardProducts>(dataSet.Tables[0]);
        }

        public async Task<RebateProgressDto> GetRebateProgressInfo(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRebatePointProgressInfo, paraObjects);
            return Common.ConvertDataTable<RebateProgressDto>(dataSet.Tables[0]).FirstOrDefault();
        }

    }
    public class RewardProductRepository : GenericRepository<RewardProduct>, IRewardProductService
    {
        private readonly RxFairDbContext _context;
        public RewardProductRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<RewardProductDto>> GetRewardProductListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRewardProductList, paraObjects);
            return Common.ConvertDataTable<RewardProductDto>(dataSet.Tables[0]);

        }
    }

    public class RedeemRequestRepository : GenericRepository<RedeemRequest>, IRedeemRequestService
    {
        private readonly RxFairDbContext _context;
        public RedeemRequestRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<RedeemRequestDto>> GetRedeemRequestList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetRedeemRequest, paraObjects);
            return Common.ConvertDataTable<RedeemRequestDto>(dataSet.Tables[0]);
        }
    }

    public class RewardMonthDaysRepository : GenericRepository<RewardMonthDays>, IRewardMonthDaysService
    {
        private readonly RxFairDbContext _context;
        public RewardMonthDaysRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }

}
