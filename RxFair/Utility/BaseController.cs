using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RxFair.Dto.Global;
using RxFair.Models;
using RxFair.Service.Interface;

namespace RxFair.Utility
{
    public class BaseController<T> : Controller where T : BaseController<T>
    {
        #region Fields
        protected JsonResponse JsonResponse => new JsonResponse();

        private IOptions<GlobalRxFair> _globalRxFair;
        protected IOptions<GlobalRxFair> GlobalRxFair => _globalRxFair ?? (_globalRxFair = HttpContext.RequestServices.GetService<IOptions<GlobalRxFair>>());

        private IErrorLogService _errorLog;
        protected IErrorLogService ErrorLog => _errorLog ?? (_errorLog = HttpContext.RequestServices.GetService<IErrorLogService>());

        private IHttpContextAccessor _accessor;
        protected IHttpContextAccessor Accessor => _accessor ?? (_accessor = HttpContext.RequestServices.GetService<IHttpContextAccessor>());

        private IHostingEnvironment _hostingEnvironment;
        protected IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = HttpContext.RequestServices.GetService<IHostingEnvironment>());

        private IMapper _mapper;
        protected IMapper Mapper => _mapper ?? (_mapper = HttpContext.RequestServices.GetService<IMapper>());

        private IConfiguration _config;
        protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        #endregion

        public string GetSortingColumnName(int sortColumnNo)
        {
            return Accessor.HttpContext.Request.Query["mDataProp_" + sortColumnNo][0];
        }

        public string GetPhysicalUrl()
        {
            return Config.GetValue<string>("CommonProperty:PhysicalUrl");
        }

        public string GetClientAppUrl()
        {
            return Config.GetValue<string>("CommonProperty:ClientAppUrl");
        }

        public int GetDefaultSubscriptionPlanDuration()
        {
            return Config.GetValue<int>("CommonProperty:DefaultSubscriptionDuration");
        }

        public string GetConfigValue(string key)
        {
            return Config.GetValue<string>(key);
        }

        public string GetS3ServiceUrl(string buketName, string fileName)
        {
            return $@"{Config.GetValue<string>("CommonProperty:S3ServiceUrl").Replace("{buketname}", buketName)}{fileName}";
        }
    }
}