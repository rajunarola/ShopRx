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
    public class SubscriptionCommissionCalculationEndOfMonth
    {
        private readonly EmailService _emailService;
        private readonly ICommissionHistoryService _commissionHistory;
        private readonly IDistributorOrderChargeService _distributorOrder;

        public SubscriptionCommissionCalculationEndOfMonth(IOptions<EmailSettingsGmail> emailSettingsGmail, ICommissionHistoryService commissionHistory, IDistributorOrderChargeService distributorOrder)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _commissionHistory = commissionHistory;
            _distributorOrder = distributorOrder;
        }

        [FunctionName("SubscriptionCommissionCalculationEndOfMonth")]
        public async Task Run([TimerTrigger("0 0 12 1 * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            //Do stuff every month at 1st date at 12AM - 0 0 12 1 * * 
            if (myTimer.IsPastDue)
            {
                try
                {
                    var parameter = new List<SqlParameter>();
                    var commissionList = await _commissionHistory.GetPendingCommissionCalculation();
                    foreach (var item in commissionList)
                    {
                        CommissionHistory commissionHistory = new CommissionHistory
                        {
                            DistributorId = item.DistributorId,
                            CommissionAmount = item.CommissionAmount,
                            CommissionStatus = (short)GlobalEnums.PaymentStatus.Pending,
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        };
                        await _commissionHistory.InsertAsync(commissionHistory, null);

                        var distributorOrderCharge = _distributorOrder.GetById(item.Id);
                        if (distributorOrderCharge != null)
                        {
                            distributorOrderCharge.CommissionCountDate = DateTime.UtcNow;
                            await _distributorOrder.UpdateAsync(distributorOrderCharge, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);
                }
            }

        }
    }
}
