using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface;
using RxFair.Service.Utility;

namespace Rxfair.AzureFunction
{
    public class WatchListPrice
    {
        private readonly IWatchlistService _watchlistService;
        private readonly EmailService _emailService;

        public WatchListPrice(IWatchlistService watchlistService, IOptions<EmailSettingsGmail> emailSettingsGmail)
        {
            _watchlistService = watchlistService;
            _emailService = new EmailService(emailSettingsGmail);
        }

        [FunctionName("WatchListPrice")]
        public void Run([TimerTrigger("0 0 12 * * *", RunOnStartup = false)]TimerInfo myTimer, ILogger log)
        {
            //Do stuff every day at 12AM - 0 0 12 * * * 
            if (myTimer.IsPastDue)
            {
                try
                {
                    var result = _watchlistService.GetAll();
                    foreach (var item in result)
                    {
                        var prices = item.MedicineMaster.MedicinePriceMasters.Where(x => x.DistributorId != null).ToList();
                        foreach (var priceItem in prices)
                        {
                            if (priceItem.WacpackagePrice == item.MatchPrice || priceItem.WacpackagePrice <= item.MatchPrice)
                            {
                                log.LogInformation("---------Succeed--------");
                                //await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                                //{
                                //    BodyText = @"<h1>Test Mail</h1>",
                                //    Subject = "Test Mail",
                                //    ToAddress = "anc@narola.email",
                                //    ToDisplayName = "Anc Narola"
                                //});
                            }
                            else
                            {
                                log.LogInformation("---------Not Succeed--------");
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
    }
}
