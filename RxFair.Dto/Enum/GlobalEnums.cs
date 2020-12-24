using System.ComponentModel;

namespace RxFair.Dto.Enum
{
    public class GlobalEnums
    {
        public enum PharmacyStatus
        {
            Pending = 1,
            Accepted = 2,
            Rejected = 3,
            Deleted = 4
        }

        public enum SubscriptionTypes
        {
            [Description("Silver")]
            Silver = 1,
            [Description("Gold")]
            Gold = 2,
            [Description("Platinum")]
            Platinum = 3
        }

        public enum UserGroup
        {
            [Description("Admin")]
            Admin = 1,

            [Description("Distributor")]
            Distributor = 2,

            [Description("Pharmacy")]
            Pharmacy = 3
        }

        public enum DocumentType
        {
            [Description("Credit Application & Terms and Conditions")]
            CreditApplicationTermsConditions = 1,

            [Description("Distributor Return Policy")]
            ReturnPolicy = 2,

            [Description("Distributor License Certificates")]
            LicenseCertificates = 3,

            [Description("Waiver")]
            Waiver = 4
        }

        public enum RewardType
        {
            [Description("Referral")]
            Referral = 1,

            [Description("New Order")]
            NewOrder = 2
        }

        public enum DeliveryType
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Send")]
            Send = 2,
            [Description("Delivered")]
            Delivered = 3
        }

        public enum MeasurementType
        {
            [Description("Strength")]
            Strength = 1,
            [Description("Package Size")]
            PackageSize = 2,
            [Description("Dose Unit")]
            DoseUnit = 3,
            [Description("Brand")]
            Brand = 4,
            [Description("Package Description Code")]
            PackageDescriptionCode = 5
        }

        public enum DealType
        {
            [Description("TopDeals")]
            TopDeals = 1,
            [Description("DealOfTheDay")]
            DealOfTheDay = 2,
            [Description("ProductPriceIncrease")]
            ProductPriceIncrease = 3
        }

        public enum AdvertisementLimit
        {
            [Description("GoldSubscriptionLimit")]
            GoldSubscriptionLimit = 30,

            [Description("GoldSubscriptionMedicineLimit")]
            MeidicineLimit = 30
        }

        public enum OrderStatus
        {
        
            Pending = 0,
            Confirmed = 1,
            Shipped = 2,
            Delivered = 3,
            Cancelled = 4,
            Returned = 5
        }

        public enum PaymentStatus
        {
            Pending = 1,
            Partial = 2,
            Completed = 3
        }
    }
}
