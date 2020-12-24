using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RxFair.Data.DbModel;

namespace RxFair.Data.DbContext
{
    public class RxFairDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public RxFairDbContext(DbContextOptions<RxFairDbContext> options)
            : base(options)
        {
            //Database.Migrate();
        }

        public virtual DbSet<AdvertiseEmailTemplate> AdvertiseEmailTemplate { get; set; }
        public virtual DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<AdvertisementMedicine> AdvertisementMedicines { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<BlogCategory> BlogCategory { get; set; }
        public virtual DbSet<BlogImage> BlogImage { get; set; }
        public virtual DbSet<BlogTag> BlogTags { get; set; }

        public virtual DbSet<ContactDetails> ContactDetails { get; set; }
        public virtual DbSet<ContactRequest> ContactRequest { get; set; }
        public virtual DbSet<CommissionHistory> CommissionHistories { get; set; }

        public virtual DbSet<Distributor> Distributor { get; set; }
        public virtual DbSet<DistributorDocumentMaster> DistributorDocumentMasters { get; set; }
        public virtual DbSet<DistributorOrder> DistributorOrders { get; set; }
        public virtual DbSet<DistributorOrderCharge> DistributorOrderCharges { get; set; }
        public virtual DbSet<DistributorOrderSetting> DistributorOrderSetting { get; set; }
        public virtual DbSet<DistributorSubscription> DistributorSubscription { get; set; }
        public virtual DbSet<DistributorSubscriptionHistory> DistributorSubscriptionHistories { get; set; }
        public virtual DbSet<DocumentMaster> DocumentMaster { get; set; }
        public virtual DbSet<DosageFormMaster> DosageFormMaster { get; set; }

        public virtual DbSet<EmailSubscription> EmailSubscriptions { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }

        public virtual DbSet<FAQs> FaQs { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoicePayment> InvoicePayments { get; set; }

        public virtual DbSet<ManufacturerMaster> ManufacturerMaster { get; set; }
        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<MedicineCategoryMaster> MedicineCategoryMaster { get; set; }

        public virtual DbSet<MedicineImage> MedicineImages { get; set; }
        public virtual DbSet<MedicineMaster> MedicineMasters { get; set; }
        public virtual DbSet<MedicinePriceMaster> MedicinePriceMasters { get; set; }
        public virtual DbSet<MedicinePriceHistory> MedicinePriceHistory { get; set; }

        public virtual DbSet<NewDistributorRequest> NewDistributorRequest { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Pharmacy> Pharmacy { get; set; }
        public virtual DbSet<PharmacyShippingAddress> PharmacyShippingAddress { get; set; }
        public virtual DbSet<PharmacyBillingAddress> PharmacyBillingAddress { get; set; }
        public virtual DbSet<PharmacySystemMaster> PharmacySystemMaster { get; set; }
        public virtual DbSet<PharmacyTypeMaster> PharmacyTypeMaster { get; set; }

        public virtual DbSet<RedeemRequest> RedeemRequests { get; set; }
        public virtual DbSet<RewardEarn> RewardEarns { get; set; }
        public virtual DbSet<RewardMoneyMaster> RewardMoneyMaster { get; set; }
        public virtual DbSet<RewardMonthDays> RewardMonthDays { get; set; }
        public virtual DbSet<RewardProduct> RewardProducts { get; set; }
        public virtual DbSet<RewardTypeMaster> RewardTypeMaster { get; set; }
        public virtual DbSet<RxExtDbMedicine> RxExtDbMedicines { get; set; }

        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<SubscriptionType> SubscriptionType { get; set; }
        public virtual DbSet<SubscriptionTypeHistory> SubscriptionTypeHistories { get; set; }

        public virtual DbSet<TermsAndCondition> TermsAndCondition { get; set; }
        public virtual DbSet<Testimonials> Testimonials { get; set; }
        public virtual DbSet<TimeZoneMaster> TimeZoneMaster { get; set; }

        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        public virtual DbSet<UploadedMedicine> UploadedMedicines { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<DistributorMedicine> DistributorMedicines { get; set; }
        public virtual DbSet<CategoryCommission> CategoryCommissions { get; set; }
        public virtual DbSet<OrderWiseReward> OrderWiseRewards { get; set; }

        #region Access Permission
        public virtual DbSet<SystemModule> SystemModules { get; set; }
        public virtual DbSet<RolesModuleAccess> RolesModuleAccesses { get; set; }
        public virtual DbSet<AccessModuleFunctionality> AccessModuleFunctionalities { get; set; }
        public virtual DbSet<Functionality> Functionalities { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            // Change Default filed datatype & length
            modelBuilder.Entity<ApplicationUser>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>().Property(c => c.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimType).HasMaxLength(50);
            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimValue).HasMaxLength(200);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(12);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }
    }


}
