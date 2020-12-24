using RxFair.Dto.Dtos;
using System.Collections.Generic;

namespace RxFair.Dto.Enum
{
    public class GlobalFormates
    {
        public const string DefaultDate = "MMM/dd/yyyy";
        public const string FullDate = "MM/dd/yyyy hh:mm tt";

    }

    public class DealType
    {
        public const string TopDeals = "TopDeals";
        public const string DealOfTheDay = "DealOfTheDay";
        public const string ProductPriceIncrease = "PriceIncrease";
    }

    public class DealTypeLabels
    {
        public const string TopDeals = "Top Deals";
        public const string DealOfTheDay = "Deal Of The Day";
        public const string ProductPriceIncrease = "Price Increase";
    }

    public class MedicineInfo
    {
        public const string InStock = "InStock";
        public const string Contracted = "Contracted";
        public const string ShortDated = "ShortDated";
    }


    public class AuthorizeRoles
    {
        public const string Admin = "SystemSuperAdmin,SystemAdmin,FinanceManager,DataManager,CustomerSupportForDistributor,CustomerSupportForPharmacy";
        public const string Distributor = "DistributorPrimaryAdmin,DistributorAdmin,DistributorStaff";
        public const string Pharmacy = "PharmacyPrimaryAdmin,PharmacyAdmin,PharmacyStaff";
    }

    public class UserRoles
    {
        public const string SystemSuperAdmin = "SystemSuperAdmin";
        public const string SystemAdmin = "SystemAdmin";
        public const string FinanceManager = "FinanceManager";
        public const string DataManager = "DataManager";
        public const string CustomerSupportForDistributor = "CustomerSupportForDistributor";
        public const string CustomerSupportForPharmacy = "CustomerSupportForPharmacy";
        public const string DistributorPrimaryAdmin = "DistributorPrimaryAdmin";
        public const string DistributorAdmin = "DistributorAdmin";
        public const string DistributorStaff = "DistributorStaff";
        public const string PharmacyPrimaryAdmin = "PharmacyPrimaryAdmin";
        public const string PharmacyAdmin = "PharmacyAdmin";
        public const string PharmacyStaff = "PharmacyStaff";
        public const string Developer = "Developer";
    }

    public class UserRoleForDisplay
    {
        public const string SystemSuperAdmin = "System Super Admin";
        public const string SystemAdmin = "System Admin";
        public const string FinanceManager = "Finance Manager";
        public const string DataManager = "Data Manager";
        public const string CustomerSupportForDistributor = "Customer Support For Distributor";
        public const string CustomerSupportForPharmacy = "Customer Support For Pharmacy";
        public const string DistributorPrimaryAdmin = "Distributor Primary Admin";
        public const string DistributorAdmin = "Distributor Admin";
        public const string DistributorStaff = "Distributor Staff";
        public const string PharmacyPrimaryAdmin = "Pharmacy Primary Admin";
        public const string PharmacyAdmin = "Pharmacy Admin";
        public const string PharmacyStaff = "Pharmacy Staff";
        public const string Developer = "Narola Admin";
    }

    public class UserClaims
    {
        public const string UserId = "UserId";
        public const string PharmacyId = "PharmacyId";
        public const string DistributorId = "DistributorId";
        public const string UserRoleGroup = "UserRoleGroup";
        public const string UserRole = "UserRole";
        public const string DisplayUserRole = "DisplayUserRole";
        public const string IsSubscriptionExpired = "IsSubscriptionExpired";
        public const string SubscriptionTypeId = "SubscriptionTypeId";
        public const string LastName = "LastName";
        public const string FullName = "FullName";
        public const string UserProfileImage = "UserProfileImage";
    }

    public class UserRoleGroup
    {
        public const string Admin = "Admin";
        public const string Distributor = "Distributor";
        public const string Pharmacy = "Pharmacy";
        public const string Developer = "Developer";
    }

    public class BucketName
    {
        public const string MedicineImage = "medicine-image";
        
    }



    public class EmailTemplateList
    {
        public const string EmailTemplate = @"EmailTemplate\";
    }

    public class EmailTagMessageList
    {

        public const string OverNightChargeLabel = @"OverNight Charge:";
        public const string OverNightChargeNote = @" NOTE : In this order, Overnight charges are included because you have ordered after distributors cutoff time.";
    }

    public class EmailTemplateFileList
    {
        public const string CreateDistributor = @"DistributorCreate.html";
        public const string SubscriptionActivation = @"SubscriptionActivation.html";
        public const string ResetPassword = @"ResetPassword.html";
        public const string PharmacyStatus = @"PharmacyStatus.html";
        public const string NewUser = @"NewUser.html";
        public const string MedicineRequestStatus = @"MedicineRequestStatus.html";
        public const string NewPharmacy = @"NewPharmacy.html";
        public const string Confirmation = @"Confirmation.html";
        public const string CreateNewOrder = @"CreateNewOrder.html";
        public const string UpdateOrder = @"UpdateOrder.html";
        public const string InnerNewOrder = @"InnerNewOrder.html";
        public const string UpdateRole = @"UpdateRole.html";
        public const string InvoiceEmail = @"InvoiceEmail.html";
        public const string InnerInvoice = @"InnerInvoice.html";
        public const string CommissionInvoice = @"CommissionInvoice.html";
        public const string MedicineDetails = @"MedicineDetailInvoice.html";
        public const string EndSubscription = @"EndSubscription.html";
        public const string PharmacyRedeemRequest = @"PharmacyRedeemRequest.html";
        public const string PharmacyCongratulation = @"PharmacyCongratulation.html";
        public const string Subscription = @"Subscription.html";

    }

    public class FilePathList
    {
        public const string Pharmacy = @"UploadFile\Pharmacy";
        public const string UserProfile = @"UploadFile\UserProfile";
        public const string Testimonial = @"UploadFile\Testimonial";
        public const string Document = @"UploadFile\Document";
        public const string Invoice = @"UploadFile\Invoice";
        public const string BlogImage = @"UploadFile\BlogImage";
        public const string RewardProduct = @"UploadFile\RewardProduct";
        public const string MedicineImage = @"UploadFile\MedicineImage";
    }

    public class BlobStorageContainers
    {
        public const string MedicineImage = @"rxfair-medicineimage";
    }

    public class TokenProvider
    {
        public const string DataProtector = "DataProtectorTokenProvider";
    }

    public class TokenPurpose
    {
        public const string Payment = "Payment";
    }

    public class StoredProcedureList
    {
        public const string GetAdvertiseList = @"GetAdvertiseTemplateList";
        public const string GetBlogCategoryList = @"GetBlogCategoryList";
        public const string GetBlogList = @"GetBlogList";
        public const string GetContactRequestList = @"GetContactRequestList";
        public const string GetSubscriptionTypeHistoryList = @"GetSubscriptionTypeHistoryList";
        public const string GetDistributorList = @"GetDistributorList";
        public const string GetDistributorUserList = @"GetDistributorUserList";
        public const string GetDistributorSubscriptionList = @"GetDistributorSubscriptionList";
        public const string GetDistributorSubscriptionHistoryList = @"GetDistributorSubscriptionHistoryList";
        public const string GetDistributorAdminList = @"GetDistributorAdminList";
        public const string GetAllNotSubscriptionDistributorList = @"GetAllNotSubscriptionDistributorList";
        public const string GetDocumentList = @"GetDocumentList";
        public const string GetDosageFormList = @"GetDosageFormList";
        public const string GetFaQsList = @"GetFaQsList";
        public const string GetFaQsListBySearch = @"GetFaQsListBySearch";
        public const string GetManufacturerList = @"GetManufacturerList";
        public const string GetMedicineCategoryList = @"GetMedicineCategoryList";
        public const string GetNewPharmacyRequestList = @"GetNewPharmacyRequestList";
        public const string GetNewDistributorRequestList = @"GetNewDistributorRequestList";
        public const string GetPharmacyAdmin = @"GetPharmacyAdmin";
        public const string GetPharmacyUserList = @"GetPharmacyUserList";
        public const string GetRecentBlogs = @"GetRecentBlogs";
        public const string GetSubscriptionTypeList = @"GetSubscriptionTypeList";
        public const string GetSystemModuleList = @"GetSystemModuleList";
        public const string GetTestimonialsList = @"GetTestimonialsList";
        public const string GetUserList = @"GetUserList";
        public const string GetAdminDashboard = @"GetAdminDashboard";
        public const string GetUserAllowedMenuList = @"GetUserAllowedMenu";
        public const string GetRolesModuleAccess = @"GetRolesModuleAccess";
        public const string GetRolesWishModuleAccess = @"GetRolesWishModuleAccess";
        public const string GetUserGroupWishRole = @"GetUserGroupWishRole";
        public const string GetRewardMoneyList = @"GetRewardMoneyList";
        public const string GetRewardProductList = @"GetRewardProductList";
        public const string GetMoneyRangeList = @"GetMoneyRangeList";
        public const string GetRewardEarnHistoryList = @"GetRewardEarnHistoryList";
        public const string GetEarnMoneyList = @"GetEarnMoneyList";
        public const string GetPharmacyDistributorDashboard = @"GetPharmacyDistributorDashboard";
        public const string GetRewardEarnProductList = @"GetRewardEarnProductList";
        public const string RewardCalculation = @"RewardCalculation";
        public const string GetPendingCommissionCalculation = @"GetPendingCommissionCalculation";
        public const string GetRedeemRequest = @"GetRedeemRequest";
        public const string GetEarnedRewardProductList = @"GetEarnedRewardProductList";
        public const string GetMeasurementList = @"GetMeasurementList";
        public const string GetAdvertisementList = @"GetAdvertisementList";
        public const string GetAdvertisementRequestList = @"GetAdvertisementRequestList";
        public const string GetMedicineList = @"GetMedicineList";
        public const string GetUploadedMedicineList = @"GetUploadedMedicineList";
        public const string GetPharmacyAdvertisements = @"GetPharmacyAdvertisements";
        public const string GetPharmacyMedicines = @"GetPharmacyMedicines";
        public const string GetAdvertisementMedicine = @"GetAdvertisementMedicine";
        public const string GetSystemMedicineList = @"GetSystemMedicineList";
        public const string GetWatchListDetails = @"GetWatchListDetails";
        public const string GetWatchList = @"GetWatchList";
        public const string GetWatchListOnSearch = @"GetWatchListOnSearch";
        public const string ViewMedicineInfo = @"ViewMedicineInfo";
        public const string GetDistributorMySellMedicineList = @"GetDistributorMySellMedicineList";
        public const string GetDistributorSystemMedicineList = @"GetDistributorSystemMedicineList";
        public const string SearchMedicineList = @"SearchMedicine";
        public const string GetMedicineNameList = @"GetMedicineNameList";
        public const string GetDuplicateMedicineList = @"GetDuplicateMedicineList";
        public const string GetCartList = @"GetCartList";
        public const string GetDistributorOrderSetting = @"GetDistributorOrderSetting";
        public const string GetMedicineHistory = @"GetMedicineHistory";
        public const string GetMedicineHistoryById = @"GetMedicineHistoryById";
        public const string GetPharmacyPurchaseOrder = @"PharmacyPurchaseOrder";
        public const string GetPharmacyPurchaseDistributorOrder = @"GetPharmacyPurchaseDistributorOrder";
        public const string GetDistributorSalesOrder = @"GetDistributorSalesOrder";
        public const string GetReviewMedicinePrice = @"ReviewMedicinePrice";
        public const string GetReviewMedicineDistributorWise = @"ReviewMedicinePriceDistributorWise";
        public const string GetPharmacyOrderSummary = @"PharmacyOrderSummary";
        public const string GetUnshippedOrders = @"UnshippedOrders";
        public const string GetSalesOrderReport = @"SalesOrderReport";
        public const string GetOrders = @"ManageOrder";
        public const string GetOrdersDetails = @"ManageOrderDetails";
        public const string PharmacyOrderList = @"PharmacyOrderList";
        public const string GetOrderDistributorList = @"GetOrderDistributorList";
        public const string GetPendingOrderList = @"GetPendingOrderList";
        public const string GetAllOrderList = @"GetAllOrderList";
        public const string GetDistributorOrderChargeMedList = @"GetDistributorOrderChargeMedList";
        public const string GetMedicinePurchaseHistory = @"GetMedicinePurchaseHistory";
        public const string GetDistributorCommissionList = @"GetDistributorCommissionList";
        public const string GetCommission = @"GetCommission";
        public const string GetCommissionInvoicePaymentList = @"GetCommissionInvoicePaymentList";
        public const string GetPendingCommssionIdlist = @"GetPendingCommssionIdlist";
        public const string GetPendingPaymentlist = @"GetPendingPaymentlist";
        public const string GetDistributorCommissionInvoiceList = @"GetDistributorCommissionInvoiceList";
        public const string MedicineSearch = @"MedicineSearch";
        public const string GetDistributorSubInfoList = @"GetDistributorSubInfo";
        public const string GetRebatePointProgressInfo = @"GetRebatePointProgressInfo";
    }

    public class TempUserGroup {
        public const string SystemUser = @"System";
    }   

}
