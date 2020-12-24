using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PayPal.Api;

namespace RxFair.Utility.Helpers
{
    public interface IPayPalService
    {
        APIContext GetApiContext();
        string GetAccessToken();
        Dictionary<string, string> GetConfig();
        string GetUniqueNumber();
    }
    public class PaypalAppSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RequestRetries { get; set; }
        public string ConnectionTimeout { get; set; }
        public string Mode { get; set; }
    }

    public class PaypalService : IPayPalService
    {
        public readonly string ClientId;
        public readonly string ClientSecret;
        private IOptions<PaypalAppSettings> _paypalAppSettings;

        public PaypalService(IOptions<PaypalAppSettings> paypalAppSettings)
        {
            _paypalAppSettings = paypalAppSettings;
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }

        public Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>() {
                //{ "clientId", "AaDjD-Wy0m1jKQCLOvr4lQa1cvAwhbD8j8JH2QxzUUf4XxoVPx2LXhdNjNLaSlxlTHNyVB5MebM7NwwI" },
                //{ "clientSecret", "ELG2FJBHm7hV8i5SNcZjLSc0GGm7cnUWW3E93Snf0e3hJDBh3G819tT5CmR_v6F6vAmiFoVX_uZoiFxt" },
                //{ "requestRetries", "1" },
                //{ "connectionTimeout", "360000" },
                //{ "mode", "sandbox" }
                { "clientId", _paypalAppSettings.Value.ClientId },
                { "clientSecret", _paypalAppSettings.Value.ClientSecret },
                { "requestRetries", _paypalAppSettings.Value.RequestRetries },
                { "connectionTimeout", _paypalAppSettings.Value.ConnectionTimeout },
                { "mode", _paypalAppSettings.Value.Mode }
            };

        }
        public string GetAccessToken()
        {
            // getting accesstocken from paypal                
            var accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();


            //Dictionary<string, string> payPalConfig = new Dictionary<string, string>();
            //payPalConfig.Add("mode", "live");
            //OAuthTokenCredential tokenCredential = new OAuthTokenCredential("AV5oGuN0AuLhPxYX4XDBaWfQxZgSEyhKPK8VkA0pP4NeBrMQmtVeTWBN4IjI5AyVidzumxvCUTXNQdR5", "EDTGT2YSNDgRmrsjrKQ3LHHJSznJJQbLCni5FKnLadCE_ZOebIXBiOXaUpDCBM7fYbIW-ZI-W6Gnw82A", payPalConfig);
            //string accessToken = tokenCredential.GetAccessToken();
            return accessToken;
        }

        public APIContext GetApiContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken()) { Config = GetConfig() };
            return apiContext;
        }
        public string GetUniqueNumber()
        {
            return DateTime.Now.ToString("ddMMyyyyHHmmss");
        }


    }

}
