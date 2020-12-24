using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rxfair.AzureFunction;
using RxFair.Data.DbContext;
using RxFair.Service.Implemetation;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Rxfair.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            builder.Services.AddDbContext<RxFairDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            builder.Services.Configure<EmailSettingsGmail>(config.GetSection("EmailSettingsGmail"));
            builder.Services.AddScoped<IWatchlistService, WatchListRepository>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementRepository>();
            builder.Services.AddScoped<IAdvertisementMedicineService, AdvertisementMedicineRepository>();
            builder.Services.AddScoped<IOrderService, OrderRepository>();
            builder.Services.AddScoped<IPharmacyService, PharmacyRepository>();
            builder.Services.AddScoped<IDistributorOrderChargeService, DistributorOrderChargeRepository>();
            builder.Services.AddScoped<IRewardMonthDaysService, RewardMonthDaysRepository>();
            builder.Services.AddScoped<IRewardEarnService, RewardEarnRepository>();
            builder.Services.AddScoped<IRewardMoneyMasterService, RewardMoneyMasterRepository>();
            builder.Services.AddScoped<ICommissionHistoryService, CommissionHistoryRepository>();
        }

    }
}
