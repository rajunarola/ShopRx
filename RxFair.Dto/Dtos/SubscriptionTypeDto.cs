namespace RxFair.Dto.Dtos
{
    public class SubscriptionTypeDto : BaseModel
    {
        public string SubscriptionTypeName { get; set; }
        public decimal ChargedMonthly { get; set; }
        public decimal SubscriptionCharge { get; set; }
        public string Description { get; set; }
        public decimal Brand { get; set; }
        public decimal Generic { get; set; }
        public decimal Otc { get; set; }
    }

    public class SubscriptionTypeHistoryDto : BaseModel
    {
        public long SubscriptionTypeId { get; set; }

        public string SubscriptionTypeName { get; set; }

        public decimal SubscriptionCharge { get; set; }

        public decimal ChargedMonthly { get; set; }

        public decimal Brand { get; set; }

        public decimal Generic { get; set; }

        public decimal Otc { get; set; }
    }
}
