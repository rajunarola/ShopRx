using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RxFair.Dto.Dtos;

namespace RxFair.Service.Utility
{
    public class EmailService
    {
        private readonly EmailSettings _settings;
        private readonly EmailSettingsGmail _settingsGmail;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public EmailService(IOptions<EmailSettingsGmail> settingsGmail)
        {
            _settingsGmail = settingsGmail.Value;
        }

        public EmailService(IOptions<EmailSettings> settings, IOptions<EmailSettingsGmail> settingsGmail)
        {
            _settings = settings.Value;
            _settingsGmail = settingsGmail.Value;
        }

        /**Send mail using Send Grid **/
        public async Task SendMailsAsyncBySendGrid(List<SendEmailModel> sendEmailModelList)
        {
            //sendgrid function
            if (_settings.IsSendGrid)
            {
                foreach (var sendEmailModel in sendEmailModelList)
                {
                    var uname = _settings.SmtpUserName;
                    var email = _settings.SmtpSenderEmail;
                    var password = _settings.SmtpPassword;
                    var host = _settings.SmtpHost;
                    var port = _settings.SmtpPort;
                    var enableSsl = _settings.SmtpEnableSsl;
                    var companyName = "RxFair";
                    //bcc = RequestHelpers.GetConfigValue("CompanyEmailBCC");
                    var message = new MailMessage { From = new MailAddress(email, companyName) };
                    if (sendEmailModel.ToAddress.Contains(","))
                    {
                        foreach (var address in sendEmailModel.ToAddress.Split(','))
                        {
                            message.To.Add(new MailAddress(address));
                        }
                    }
                    else
                    {
                        message.To.Add(new MailAddress(sendEmailModel.ToAddress));
                    }
                    if (!string.IsNullOrEmpty(sendEmailModel.CcAddress)) message.CC.Add(sendEmailModel.CcAddress);


                    message.IsBodyHtml = true;
                    // Subject and multipart/alternative Body
                    message.Subject = sendEmailModel.Subject;
                    message.Body = sendEmailModel.BodyText;
                    message.Priority = MailPriority.High;

                    var loginInfo = new NetworkCredential(uname, password);
                    // Init SmtpClient and send
                    var smtpClient = new SmtpClient(host, port)
                    {
                        EnableSsl = enableSsl,
                        UseDefaultCredentials = false,
                        Credentials = loginInfo,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    await smtpClient.SendMailAsync(message);
                }
            }
            else //smtp function
            {
                foreach (var sendEmailModel in sendEmailModelList)
                {
                    var uname = _settingsGmail.SmtpUserName;
                    var email = _settingsGmail.SmtpSenderEmail;
                    var password = _settingsGmail.SmtpPassword;
                    var host = _settingsGmail.SmtpHost;
                    var port = _settingsGmail.SmtpPort;
                    var enableSsl = _settingsGmail.SmtpEnableSsl;
                    var companyName = "EAID";
                    //bcc = RequestHelpers.GetConfigValue("CompanyEmailBCC");
                    var message = new MailMessage { From = new MailAddress(email, companyName) };
                    if (sendEmailModel.ToAddress.Contains(","))
                    {
                        foreach (var address in sendEmailModel.ToAddress.Split(','))
                        {
                            message.To.Add(new MailAddress(address));
                        }
                    }
                    else
                    {
                        message.To.Add(new MailAddress(sendEmailModel.ToAddress));
                    }
                    if (!string.IsNullOrEmpty(sendEmailModel.CcAddress)) message.CC.Add(sendEmailModel.CcAddress);


                    message.IsBodyHtml = true;
                    // Subject and multipart/alternative Body
                    message.Subject = sendEmailModel.Subject;
                    message.Body = sendEmailModel.BodyText;
                    message.Priority = MailPriority.High;

                    var loginInfo = new NetworkCredential(uname, password);
                    // Init SmtpClient and send
                    var smtpClient = new SmtpClient(host, port)
                    {
                        EnableSsl = enableSsl,
                        UseDefaultCredentials = false,
                        Credentials = loginInfo,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    await smtpClient.SendMailAsync(message);
                }
            }
        }

        public async Task SendMailAsyncBySendGrid(SendEmailModel sendEmailModel)
        {
            //sendgrid function
            if (_settings.IsSendGrid)
            {
                var uname = _settings.SmtpUserName;
                var email = _settings.SmtpSenderEmail;
                var password = _settings.SmtpPassword;
                var host = _settings.SmtpHost;
                var port = _settings.SmtpPort;
                var enableSsl = _settings.SmtpEnableSsl;
                var companyName = _settings.SenderName;

                var message = new MailMessage { From = new MailAddress(email, companyName) };
                if (sendEmailModel.ToAddress.Contains(","))
                {
                    foreach (var address in sendEmailModel.ToAddress.Split(','))
                    {
                        message.To.Add(new MailAddress(address));
                    }
                }
                else
                {
                    message.To.Add(new MailAddress(sendEmailModel.ToAddress));
                }
                if (!string.IsNullOrEmpty(sendEmailModel.CcAddress)) message.CC.Add(sendEmailModel.CcAddress);


                message.IsBodyHtml = true;
                message.Subject = sendEmailModel.Subject;
                message.Body = sendEmailModel.BodyText;
                message.Priority = MailPriority.High;

                var loginInfo = new NetworkCredential(uname, password);
                // Init SmtpClient and send
                var smtpClient = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = loginInfo,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };
                //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                await smtpClient.SendMailAsync(message);
            }
            else //smtp function
            {
                try
                {
                    var uname = _settingsGmail.SmtpUserName;
                    var email = _settingsGmail.SmtpSenderEmail;
                    var password = _settingsGmail.SmtpPassword;
                    var host = _settingsGmail.SmtpHost;
                    var port = _settingsGmail.SmtpPort;
                    var enableSsl = _settingsGmail.SmtpEnableSsl;
                    var companyName = _settingsGmail.SenderName;

                    var message = new MailMessage { From = new MailAddress(email, companyName) };
                    if (sendEmailModel.ToAddress.Contains(","))
                    {
                        foreach (var address in sendEmailModel.ToAddress.Split(','))
                        {
                            message.To.Add(new MailAddress(address));
                        }
                    }
                    else
                    {
                        message.To.Add(new MailAddress(sendEmailModel.ToAddress));
                    }
                    if (!string.IsNullOrEmpty(sendEmailModel.CcAddress)) message.CC.Add(sendEmailModel.CcAddress);


                    message.IsBodyHtml = true;
                    // Subject and multipart/alternative Body
                    message.Subject = sendEmailModel.Subject;
                    message.Body = sendEmailModel.BodyText;
                    message.Priority = MailPriority.High;

                    var loginInfo = new NetworkCredential(uname, password);
                    // Init SmtpClient and send
                    var smtpClient = new SmtpClient(host, port)
                    {
                        EnableSsl = enableSsl,
                        UseDefaultCredentials = false,
                        Credentials = loginInfo,
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };
                    await smtpClient.SendMailAsync(message);
                }
                catch (Exception ex)
                {

                }
            }

        }

        /**This is temprory function to send email using gmail**/
        public async Task SendEmailAsyncByGmail(SendEmailModel model)
        {
            var uname = _settingsGmail.SmtpUserName;
            var email = _settingsGmail.SmtpSenderEmail;
            var password = _settingsGmail.SmtpPassword;
            var host = _settingsGmail.SmtpHost;
            var port = _settingsGmail.SmtpPort;
            var enableSsl = _settingsGmail.SmtpEnableSsl;
            var companyName = _settingsGmail.SenderName;


            //bcc = RequestHelpers.GetConfigValue("CompanyEmailBCC");
            var message = new MailMessage { From = new MailAddress(email, companyName) };
            if (model.ToAddress.Contains(","))
            {
                foreach (var address in model.ToAddress.Split(','))
                {
                    message.To.Add(new MailAddress(address));
                }
            }
            else
            {
                message.To.Add(new MailAddress(model.ToAddress));
            }
            if (!string.IsNullOrEmpty(model.CcAddress)) message.CC.Add(model.CcAddress);


            message.IsBodyHtml = true;
            // Subject and multipart/alternative Body
            message.Subject = model.Subject;
            message.Body = model.BodyText;
            if (model.Attachment != null)
            {
                Stream stream = new MemoryStream(model.Attachment);
                Attachment at = new Attachment(stream, model.AttachmentName);
                message.Attachments.Add(at);
            }
            message.Priority = MailPriority.High;

            var loginInfo = new NetworkCredential(uname, password);
            // Init SmtpClient and send
            var smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = loginInfo,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            await smtpClient.SendMailAsync(message);
        }

    }

    public class EmailSettings
    {
        public string SmtpUserName { get; set; }
        public string SmtpSenderEmail { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpKey { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string SenderName { get; set; }
        public bool IsSendGrid { get; set; }
    }


    public class EmailSettingsGmail
    {
        public string SmtpUserName { get; set; }
        public string SmtpSenderEmail { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public string SmtpKey { get; set; }
        public int SmtpPort { get; set; }
        public string SenderName { get; set; }
        public bool SmtpEnableSsl { get; set; }
    }

}
