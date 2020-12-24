using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RxFair.StandardLibrary
{
    public class MonthlyServices
    {
        private static StoredProcedureRepositoryBase _repositoryBase;
        private static SchedulerEmailService _schedulerEmail;

        public MonthlyServices()
        {
            _repositoryBase = new StoredProcedureRepositoryBase();
            _schedulerEmail = new SchedulerEmailService(new EmailSettingsGmail
            {
                SmtpUserName = "narolamma@gmail.com",
                SmtpHost = "smtp.gmail.com",
                SmtpSenderEmail = "narolamma@gmail.com",
                SmtpPassword = "Password123#",
                //"SmtpSenderName": "RxFair",
                SmtpPort = 587,
                SenderName = "Shop Rx",
                SmtpEnableSsl = true
            });
        }

        public void SubscriptionCommissionServiceAsync(CancellationTokenSource tokenSource)
        {
            try
            {
                string qry = StoredProceduresList.ServiceGetPendingCommission;
                //var samplemailadds = "ppt@narola.email" + "," + "ams@narola.email";
                Task.Run(async () =>
                {
                    await _repositoryBase.GetExecuteScaler<int>(qry, null, CommandType.StoredProcedure);
                });

                //Task.Run(() => _schedulerEmail.SendEmailAsyncByGmail(new SendEmailModel()
                //{
                //    BodyText = "Hello,<br/> Test mail.",
                //    Subject = "Subscription Commission Service",
                //    ToAddress = samplemailadds,
                //    ToDisplayName = "Commission Service"++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++9
                //}));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void RewardCalculationServiceAsync(CancellationTokenSource tokenSource)
        {
            try
            {
                string qry = StoredProceduresList.ServiceRewardCalculation;
                DataSet result = _repositoryBase.GetQueryDatatableAsync(qry, null, CommandType.StoredProcedure);
                var pharmacyList = _repositoryBase.CreateListFromTable<PharmacyOrder>(result.Tables[0]).GroupBy(x => x.PharmacyId).ToList();

                //foreach (var item in pharmacyList)
                //{
                //    var lastOrderDays = 0;
                //    var lastOrderDate = _order.GetAll(x => x.PharmacyId == item.Key).OrderByDescending(x => x.OrderDate).FirstOrDefault();
                //    if (lastOrderDate != null)
                //    {
                //        lastOrderDays = (int)(DateTime.Today - lastOrderDate.OrderDate).TotalDays;
                //    }
                //    foreach (var pharmacyOrder in item)
                //    {
                //        var rewardMonthDays = _rewardMonth.GetAll().FirstOrDefault();
                //        var checkRange = rangeList.FirstOrDefault(x => x.MinRange <= pharmacyOrder.SubTotal && x.MaxRange >= pharmacyOrder.SubTotal);
                //        if (checkRange != null)
                //        {
                //            var finalRewardPer = CalculateRewardPercentage(rewardMonthDays.NoOfDays, lastOrderDays, checkRange.Referral);
                //            var rewardAmount = (pharmacyOrder.SubTotal * finalRewardPer) / 100;
                //            var rewardEarn = new RewardEarn
                //            {
                //                PharmacyId = pharmacyOrder.PharmacyId,
                //                OrderId = pharmacyOrder.OrderId,
                //                RewardTypeId = (long)GlobalEnums.RewardType.NewOrder,
                //                RewardMoney = rewardAmount
                //            };
                //            await _rewardEarn.InsertAsync(rewardEarn, null);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
