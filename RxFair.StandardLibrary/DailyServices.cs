using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RxFair.StandardLibrary
{
    public class DailyServices
    {
        private static StoredProcedureRepositoryBase _repositoryBase;
        private static SchedulerEmailService _schedulerEmail;

        public DailyServices()
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

        //public void WatchListServiceAsync(CancellationTokenSource tokenSource)
        //{
        //    try
        //    {
        //        string qry = StoredProceduresList.ServiceGetWatchListPrice;
        //        DataSet result = _repositoryBase.GetQueryDatatableAsync(qry, null, CommandType.StoredProcedure);
        //        var watchList = _repositoryBase.CreateListFromTable<WatchList>(result.Tables[0]).GroupBy(x => x.PharmacyId).ToList();

        //        var samplemails = "ppt@narola.email";
        //        string physicalUrl = ConfigurationManager.AppSettings["PhysicalUrl"];
        //        string basePath = ConfigurationManager.AppSettings["BasePath"];

        //        string EmailTemplate = _schedulerEmail.ReadEmailTemplate(basePath, "WatchListMatchPrice.html", "");
        //        string MedicineDetail = _schedulerEmail.ReadEmailTemplate(basePath, "WatchListMedicineDetail.html", "");
        //        //string Emailurl = "E:\\Projects\\RxFair-TFS\\RxFair.WindowsService\\EmailTemplate\\";
        //        //string EmailTemplate = _schedulerEmail.ReadEmailTemplate($@"{Emailurl}", @"WatchListMatchPrice.html", physicalUrl);

        //        foreach (var item in watchList)
        //        {
        //            List<long> testarray = new List<long>();
        //            string mediInfoTemplate = "";
        //            string tempMedicineTemplate = "";
        //            string phyEmailTemplate = EmailTemplate;
        //            phyEmailTemplate = phyEmailTemplate.Replace("{Pharmacy Name}", item.FirstOrDefault().PharmacyName);
        //            phyEmailTemplate = phyEmailTemplate.Replace("{MedicineDate}", item.FirstOrDefault().MedicineDate.ToString("dd-MM-yyyy"));
        //            foreach (var mediItem in item)
        //            {
        //                tempMedicineTemplate = MedicineDetail;
        //                if (mediItem.WacpackagePrice == mediItem.MatchPrice || mediItem.WacpackagePrice <= mediItem.MatchPrice)
        //                {
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{ndc}", mediItem.NDC);
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{medicine name}", mediItem.MedicineName);
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{company name}", mediItem.CompanyName);
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{qty}", mediItem.Quantity.ToString());
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{current price}", mediItem.WacpackagePrice.ToString());
        //                    tempMedicineTemplate = tempMedicineTemplate.Replace("{match price}", mediItem.MatchPrice.ToString());
        //                    mediInfoTemplate = mediInfoTemplate + tempMedicineTemplate;
        //                    //mediList += $@"<tr><td>{mediItem.NDC}</td><td>{mediItem.MedicineName}</td><td>{mediItem.CompanyName}</td>"+
        //                    //"<td>{mediItem.Quantity}</td><td>{mediItem.WacpackagePrice}</td><td>{mediItem.MatchPrice}</td></tr>";
        //                    testarray.Add(mediItem.Id);
        //                }
        //                if (testarray.Any())
        //                {
        //                    string updateQry = $@"UPDATE [WatchList] SET IsNotified = 1 WHERE Id IN ({string.Join(",", testarray)});";
        //                    testarray.Clear();
        //                }
        //            }
        //            phyEmailTemplate = phyEmailTemplate.Replace("{MediList}", mediInfoTemplate);

        //            Task.Run(() => _schedulerEmail.SendEmailAsyncByGmail(new SendEmailModel()
        //            {
        //                BodyText = phyEmailTemplate,
        //                Subject = "Watch List Medicine Price Changes",
        //                ToAddress = samplemails, //item.FirstOrDefault().Email, // samplemails - Pharmacy List 
        //                ToDisplayName = item.FirstOrDefault().PharmacyName // Phamacy Name
        //            }));

        //            //var flag = new WatchList{ IsNotified = true };
        //        }
        //        //MessageBox.Show("WatchList Service Succeed !");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        throw;
        //    }
        //}

        public void WatchListServiceAsync(CancellationTokenSource tokenSource)
        {
            try
            {
                string strQuery;
                SqlCommand cmd;
                string qry = StoredProceduresList.ServiceGetWatchListPrice;
                DataSet result = _repositoryBase.GetQueryDatatableAsync(qry, null, CommandType.StoredProcedure);
                var watchList = _repositoryBase.CreateListFromTable<WatchList>(result.Tables[0]).GroupBy(x => x.PharmacyId).ToList();

                //var samplemails = "ppt@narola.email";
                string physicalUrl = ConfigurationManager.AppSettings["PhysicalUrl"];
                string basePath = ConfigurationManager.AppSettings["BasePath"];

                string EmailTemplate = _schedulerEmail.ReadEmailTemplate(basePath, "WatchListMatchPrice.html", "");
                string MedicineDetail = _schedulerEmail.ReadEmailTemplate(basePath, "WatchListMedicineDetail.html", "");
                //string Emailurl = "E:\\Projects\\RxFair-TFS\\RxFair.WindowsService\\EmailTemplate\\";
                //string EmailTemplate = _schedulerEmail.ReadEmailTemplate($@"{Emailurl}", @"WatchListMatchPrice.html", physicalUrl);

                foreach (var item in watchList)
                {
                    List<long> testarray = new List<long>();
                    string phyEmailTemplate = EmailTemplate;
                    phyEmailTemplate = phyEmailTemplate.Replace("{Pharmacy Name}", item.FirstOrDefault().PharmacyName);
                    phyEmailTemplate = phyEmailTemplate.Replace("{MedicineDate}", item.FirstOrDefault().MedicineDate.ToString("dd-MM-yyyy"));

                    string mediInfoTemplate = "";
                    foreach (var mediItem in item)
                    {
                        string tempMedicineTemplate = MedicineDetail;
                        if (mediItem.WacpackagePrice == mediItem.MatchPrice || mediItem.WacpackagePrice <= mediItem.MatchPrice)
                        {
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{ndc}", mediItem.NDC);
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{medicine name}", mediItem.MedicineName);
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{company name}", mediItem.CompanyName);
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{qty}", mediItem.Quantity.ToString());
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{current price}", mediItem.WacpackagePrice.ToString());
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{match price}", mediItem.MatchPrice.ToString());
                            mediInfoTemplate = mediInfoTemplate + tempMedicineTemplate;
                            testarray.Add(mediItem.Id);
                        }
                        if (testarray.Any())
                        {
                            strQuery = $@"UPDATE WatchList SET IsNotified = @IsNotified WHERE Id IN ({string.Join(",", testarray)});";
                            cmd = new SqlCommand(strQuery);
                            cmd.Parameters.AddWithValue("@IsNotified", true);
                            _repositoryBase.InsertUpdateData(cmd);
                            testarray.Clear();
                        }
                    }
                    // If there is no medicine in this pharmacy, It should not send mail.
                    if (!mediInfoTemplate.Equals(string.Empty))
                    {
                        phyEmailTemplate = phyEmailTemplate.Replace("{MediList}", mediInfoTemplate);
                        Task.Run(() => _schedulerEmail.SendEmailAsyncByGmail(new SendEmailModel()
                        {
                            BodyText = phyEmailTemplate,
                            Subject = "Watch List Medicine Price Changes",
                            ToAddress = item.FirstOrDefault().Email, // samplemails - Pharmacy List 
                            ToDisplayName = item.FirstOrDefault().PharmacyName // Phamacy Name
                        }));
                    }
                    //var flag = new WatchList{ IsNotified = true };
                }
                //MessageBox.Show("WatchList Service Succeed !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void DealOfTheDayServiceAsync(CancellationTokenSource tokenSource)
        {
            try
            {
                string qry = StoredProceduresList.ServiceGetDealOfTheDayReminder;
                DataSet result = _repositoryBase.GetQueryDatatableAsync(qry, null, CommandType.StoredProcedure);
                var todayAdvertisement = _repositoryBase.CreateListFromTable<DealOfTheDay>(result.Tables[1]); // Change table index while sending emails.

                string physicalUrl = ConfigurationManager.AppSettings["PhysicalUrl"];
                string basePath = ConfigurationManager.AppSettings["BasePath"];
                string EmailTemplate = _schedulerEmail.ReadEmailTemplate(basePath, "DealOfTheDay.html", "");

                //var samplemailadds = "ppt@narola.email";
                var pharmacyList = _repositoryBase.CreateListFromTable<DealOfTheDayPharmacy>(result.Tables[0]);

                foreach (var item in todayAdvertisement)
                {
                    string phyEmailTemplate = EmailTemplate;
                    phyEmailTemplate = phyEmailTemplate.Replace("{Pharmacy Name}", item.CompanyName);
                    string mediList = string.Empty;
                    mediList += $@"<tr><td>{item.NDC}</td><td>{item.MedicineName}</td><td>{item.CompanyName}</td><td>{item.DealType}</td><td>{item.DealDate.Value.ToString("dd-MM-yyyy")}</td><td>{item.DealPrice}</td></tr>";
                    phyEmailTemplate = phyEmailTemplate.Replace("{MediList}", mediList);

                    Task.Run(() => _schedulerEmail.SendEmailAsyncByGmail(new SendEmailModel()
                    {
                        BodyText = phyEmailTemplate,
                        Subject = "Deal of the day",
                        ToAddress = pharmacyList.FirstOrDefault().PharmacyName, //samplemailadds
                        ToDisplayName = pharmacyList.FirstOrDefault().PharmacyName
                    }));
                }
                //MessageBox.Show("Deal of the day Service Succeed !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
