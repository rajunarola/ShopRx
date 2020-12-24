using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using RxFair.Data.DbContext;
using RxFair.Dto.Global;
using RxFair.Service.Implemetation;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.ExcelUtility;
using RxFair.Utility.Helpers;

namespace RxFair
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);

            services.AddDbContext<RxFairDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //--< set uploadsize large files >----
            //services.Configure<FormOptions>(options =>
            //{
            //    options.ValueLengthLimit = int.MaxValue;
            //    options.MultipartBodyLengthLimit = int.MaxValue;
            //    options.MultipartHeadersLengthLimit = int.MaxValue;
            //});
            //--</ set uploadsize large files >----

            //services.ini
            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<Role>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<RxFairDbContext>();
            services.AddScoped<RoleManager<Role>>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();

            //services.AddScoped<IStorageClient>(factory => new AzureStorageClient(Configuration["AzureStorage:StorageConnection"]));
            services.AddScoped<IPayPalService, PaypalService>();

            #region Token Configuration
            //services.Configure<DataProtectionTokenProviderOptions>(options =>
            //    {
            //        options.Name = "DataProtectorTokenProvider";
            //        options.TokenLifespan = TimeSpan.FromMinutes(15);
            //    });
            #endregion

            #region Identity Configuration
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //Seting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            //Seting the Post Configure
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
             {
                 //configure your other properties
                 // Cookie settings
                 options.Cookie.HttpOnly = true;
                 options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                 options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                 options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                 options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                 options.SlidingExpiration = true;
             });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });
            #endregion

            services.AddAuthentication();

            #region Dependency Injection
            #region _A_
            services.AddScoped<IAdvertiseTemplateService, AdvertiseTemplateRepository>();
            services.AddScoped<IAdvertisementService, AdvertisementRepository>();
            services.AddScoped<IAdvertisementMedicineService, AdvertisementMedicineRepository>();
            services.AddScoped<IAccessModuleFunctionalityService, AccessModuleFunctionalityRepository>();
            #endregion

            #region _B_
            services.AddScoped<IBlogService, BlogRepository>();
            services.AddScoped<IBlogCategoryService, BlogCategoryRepository>();
            services.AddScoped<IBlogImageService, BlogImageRepository>();
            services.AddScoped<IBlogTagService, BlogTagRepository>();
            #endregion

            #region _C_
            services.AddScoped<IContactUsService, ContactUsRepository>();
            services.AddScoped<IContactRequestService, ContactRequestRepository>();
            services.AddScoped<ICartService, CartRepository>();
            services.AddScoped<ICommissionHistoryService, CommissionHistoryRepository>();
            #endregion

            #region _D_
            services.AddScoped<IDistributorService, DistributorRepository>();
            services.AddScoped<IDistributorDocumentService, DistributorDocumentRepository>();
            services.AddScoped<IDistributorSubscriptionService, DistributorSubscriptionRepository>();
            services.AddScoped<IDistributorSubscriptionHistoryService, DistributorSubscriptionHistoryRepository>();
            services.AddScoped<IDistributorOrderService, DistributorOrderRepository>();
            services.AddScoped<IDistributorOrderChargeService, DistributorOrderChargeRepository>();
            services.AddScoped<IDistributerOrderSettingService, DistributerOrderSettingRepository>();
            services.AddScoped<IDosageFormService, DosageFormRepository>();
            services.AddScoped<IDocumentService, DocumentRepository>();
            services.AddScoped<IDistributorMedicineService, DistributorMedicineRepository>();
            #endregion

            #region _E_
            services.AddScoped<IEmailSubscriptionService, EmailSubscriptionRepository>();
            services.AddScoped<IExcelReports, ExcelReports>();
            services.AddScoped<IErrorLogService, ErrorLogRepository>();
            #endregion

            #region _F_
            services.AddScoped<IFaQsService, FaQsRepository>();
            services.AddScoped<IFunctionalityService, FunctionalityRepository>();
            #endregion

            #region _G_
            #endregion

            #region _H_
            #endregion

            #region _I_
            services.AddScoped<IInvoiceService, InvoiceRepository>();
            services.AddScoped<IInvoicePaymentService, InvoicePaymentRepository>();

            #endregion

            #region _J_
            #endregion

            #region _K_
            #endregion

            #region _L_
            #endregion

            #region _M_
            services.AddScoped<IManufacturerService, ManufacturerRepository>();
            services.AddScoped<IMeasurementService, MeasurementRepository>();
            services.AddScoped<IMedicineCategoryService, MedicineCategoryRepository>();
            services.AddScoped<IMedicineImageService, MedicineImageRepository>();
            services.AddScoped<IMedicineMasterService, MedicineMasterRepository>();
            services.AddScoped<IMedicinePriceMasterService, MedicinePriceMasterRepository>();
            services.AddScoped<IMedicineHelper, MedicineHelper>();
            #endregion

            #region _N_
            services.AddScoped<INewDistributorRequestService, NewDistributorRequestRepository>();
            #endregion

            #region _O_
            services.AddScoped<IOrderService, OrderRepository>();
            #endregion

            #region _P_
            services.AddScoped<IPharmacyBillingAddressService, PharmacyBillingAddressRepository>();
            services.AddScoped<IPharmacyService, PharmacyRepository>();
            services.AddScoped<IPharmacyShippingAddressService, PharmacyShippingAddressRepository>();
            services.AddScoped<IPharmacySystemMasterService, PharmacySystemMasterRepository>();
            services.AddScoped<IPharmacyTypeMasterService, PharmacyTypeMasterRepository>();
            #endregion

            #region _Q_
            #endregion

            #region _R_
            services.AddScoped<IRedeemRequestService, RedeemRequestRepository>();
            services.AddScoped<IRewardEarnService, RewardEarnRepository>();
            services.AddScoped<IRewardTypeMasterService, RewardTypeMasterRepository>();
            services.AddScoped<IRewardMoneyMasterService, RewardMoneyMasterRepository>();
            services.AddScoped<IRewardMonthDaysService, RewardMonthDaysRepository>();
            services.AddScoped<IRewardProductService, RewardProductRepository>();
            services.AddScoped<IRolesModuleAccessService, RolesModuleAccessRepository>();
            services.AddScoped<IReportService, ReportRepository>();
            #endregion

            #region _S_
            services.AddScoped<IStateService, StateRepository>();
            services.AddScoped<ISubscriptionTypeService, SubscriptionTypeRepository>();
            services.AddScoped<ISubscriptionTypeHistoryService, SubscriptionTypeHistoryRepository>();
            services.AddScoped<ISystemModuleService, SystemModuleRepository>();
            #endregion

            #region _T_
            services.AddScoped<ITermsAndConditionService, TermsAndConditonRepository>();
            services.AddScoped<ITestimonialsService, TestimonialsRepository>();
            services.AddScoped<ITimeZoneService, TimeZoneRespository>();
            #endregion

            #region _U_
            services.AddScoped<IUserService, UserRepository>();
            services.AddScoped<IUserAddressService, UserAddressRepository>();
            services.AddScoped<IUploadedMedicineService, UploadedMedicineRepository>();
            #endregion

            #region _V_
            #endregion

            #region _W_
            services.AddScoped<IWatchlistService, WatchListRepository>();
            #endregion

            #region _X_
            #endregion

            #region _Y_
            #endregion

            #region _Z_
            #endregion 
            #endregion

            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            /**Add Automapper**/
            services.AddAutoMapper(typeof(Startup));
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddSessionStateTempDataProvider();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();
            #region Configure App Settings
            services.Configure<EmailSettingsGmail>(Configuration.GetSection("EmailSettingsGmail"));
            services.Configure<AwsS3Storage>(Configuration.GetSection("AwsS3Storage"));
            services.Configure<PaypalAppSettings>(Configuration.GetSection("PayPalAppSettings"));

            void GlobalAction(GlobalRxFair opt)
            {
                opt.CurrentTimeZone = TimeZoneInfo.Local.Id;
                opt.CurrentTimeZoneId = RxFairIdentityDataInitializer.GetTimeZoneId(opt.CurrentTimeZone, Configuration.GetConnectionString("DefaultConnection"));
                opt.IsDaylightSavingTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now);
                opt.PhysicalUrl = Configuration["CommonProperty:PhysicalUrl"];
            }

            services.Configure((Action<GlobalRxFair>)GlobalAction);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GlobalRxFair>>().Value);
            #endregion

            #region Web Jobs

            #endregion
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePages(async context =>
            {
                var statusCode = context.HttpContext.Response.StatusCode;//  contains the status code
                if (statusCode == 404 || statusCode == 501 || statusCode == 500 || statusCode == 400)
                    context.HttpContext.Response.Redirect("/Home/Error");
            });

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "Admin",
                    areaName: "Admin",
                    template: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapAreaRoute(
                    name: "Distributor",
                    areaName: "Distributor",
                    template: "Distributor/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapAreaRoute(
                    name: "Pharmay",
                    areaName: "Pharmacy",
                    template: "Pharmacy/{controller=Dashboard}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            RxFairIdentityDataInitializer.SeedData(userManager, roleManager);
            RotativaConfiguration.Setup(env);
        }
    }
}
