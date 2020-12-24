using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RxFair.Data.DbModel;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Service.Utility;

namespace Rxfair.AzureFunction
{
    public class RewardCalculationAtTheEndOfMonth
    {
        private readonly IPharmacyService _pharmacy;
        private readonly IOrderService _order;
        private readonly IDistributorOrderChargeService _distributorOrderCharge;
        private readonly IRewardMonthDaysService _rewardMonth;
        private readonly IRewardMoneyMasterService _rewardMoney;
        private readonly IRewardEarnService _rewardEarn;

        public RewardCalculationAtTheEndOfMonth(IPharmacyService pharmacy, IOrderService order, IDistributorOrderChargeService distributorOrderCharge, IRewardMonthDaysService rewardMonth, IRewardMoneyMasterService rewardMoney, IRewardEarnService rewardEarn)
        {
            _pharmacy = pharmacy;
            _order = order;
            _distributorOrderCharge = distributorOrderCharge;
            _rewardMonth = rewardMonth;
            _rewardMoney = rewardMoney;
            _rewardEarn = rewardEarn;
        }

        [FunctionName("RewardCalculationAtTheEndOfMonth")]
        public async Task Run([TimerTrigger("0 5 12 1 * *", RunOnStartup = false)]TimerInfo myTimer, ILogger log)
        {
            //Do stuff every month at 1st date at 12AM - 0 5 12 1 * * 
            if (myTimer.IsPastDue)
            {
                try
                {
                    var parameter = new List<SqlParameter>();
                    var pharmacyList = _pharmacy.GetRewardCalculation().GroupBy(x => x.PharmacyId).ToList();
                    var rangeList = _rewardMoney.GetAll(x => x.IsActive).ToList();

                    foreach (var item in pharmacyList)
                    {
                        var lastOrderDays = 0;
                        var lastOrderDate = _order.GetAll(x => x.PharmacyId == item.Key).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                        if (lastOrderDate != null)
                        {
                            lastOrderDays = (int)(DateTime.Today - lastOrderDate.OrderDate).TotalDays;
                        }
                        foreach (var pharmacyOrder in item)
                        {
                            var rewardMonthDays = _rewardMonth.GetAll().FirstOrDefault();
                            var checkRange = rangeList.FirstOrDefault(x => x.MinRange <= pharmacyOrder.SubTotal && x.MaxRange >= pharmacyOrder.SubTotal);
                            if (checkRange != null)
                            {
                                var finalRewardPer = CalculateRewardPercentage(rewardMonthDays.NoOfDays, lastOrderDays, checkRange.Referral);
                                var rewardAmount = (pharmacyOrder.SubTotal * finalRewardPer) / 100;
                                var rewardEarn = new RewardEarn
                                {
                                    PharmacyId = pharmacyOrder.PharmacyId,
                                    OrderId = pharmacyOrder.OrderId,
                                    RewardTypeId = (long)GlobalEnums.RewardType.NewOrder,
                                    RewardMoney = rewardAmount
                                };
                                await _rewardEarn.InsertAsync(rewardEarn, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);
                }
            }
        }

        private decimal CalculateRewardPercentage(int rewardMonthDays, int lastOrderDays, decimal rewardPer)
        {
            while (lastOrderDays > rewardMonthDays)
            {
                lastOrderDays = lastOrderDays - rewardMonthDays;
                rewardPer = (rewardPer / 2);
            }
            return rewardPer;
        }
    }
}
