using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Service.Utility;

namespace Rxfair.AzureFunction
{
    public class DealOfTheDayReminder
    {
        private readonly IAdvertisementMedicineService _advertisementMedicine;
        private readonly IAdvertisementService _advertisement;
        private readonly EmailService _emailService;
        private readonly IPharmacyService _pharmacy;

        public DealOfTheDayReminder(IOptions<EmailSettingsGmail> emailSettingsGmail, IAdvertisementService advertisement, IAdvertisementMedicineService advertisementMedicine, IPharmacyService pharmacy)
        {
            _advertisement = advertisement;
            _advertisementMedicine = advertisementMedicine;
            _emailService = new EmailService(emailSettingsGmail);
            _pharmacy = pharmacy;
        }

        [FunctionName("DealOfTheDayReminder")]
        public void Run([TimerTrigger("0 0 12 * * *", RunOnStartup = false)]TimerInfo myTimer, ILogger log)
        {
            //Do stuff every day at 12AM - 0 0 12 * * * 
            if (myTimer.IsPastDue)
            {
                try
                {
                    var pharmacyList = _pharmacy.GetAll(x => x.PharmacyAdminUser.EmailConfirmed && x.PharmacyAdminUser.IsActive && x.IsActive && x.Status == (int)GlobalEnums.PharmacyStatus.Accepted);
                    var todayAdvertisement = _advertisement.GetAll(x => x.DealType == (short)GlobalEnums.DealType.DealOfTheDay && x.Status.Value == true && x.DealDate.Value.Date == DateTime.Today);
                    foreach (var item in todayAdvertisement)
                    {
                        //foreach (var itemMedicine in item.AdvertisementMedicines)
                        //{
                        //    //Mail Template for sending mail to pharmacies.
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.LogInformation(ex.Message);
                }
            }
            //await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
            //{
            //    BodyText = @"<h1>Test Mail</h1>",
            //    Subject = "Test Mail",
            //    ToAddress = "anc@narola.email",
            //    ToDisplayName = "Anc Narola"
            //});
        }
    }
}
