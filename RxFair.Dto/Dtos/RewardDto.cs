using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class RewardMoneyDto : BaseModel
    {
        public long RewardTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal MinRange { get; set; }
        public decimal MaxRange { get; set; }
        public decimal Referral { get; set; }
    }

    public class RewardEarnDto : BaseModel
    {
        public long RewardTypeId { get; set; }
        public string TypeName { get; set; }
        public long? PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public long? OrderId { get; set; }
        public string Order { get; set; }
        public string RewardBy { get; set; }
        public decimal RewardMoney { get; set; }
    }

    public class RewardProductDto : BaseModel
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public IFormFile ProductImageFile { get; set; }
        public decimal Redeem { get; set; }
        public string Description { get; set; }
    }

    public class RewardMonthDaysDto
    {
        public int Id { get; set; }
        public int NoOfDays { get; set; }
    }

    public class RedeemRequestDto : BaseModel
    {
        public int IsApprove { get; set; }
        public string PharmacyName { get; set; }
        public short DeliveryStatus { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public IFormFile ProductImageFile { get; set; }
        public decimal Redeem { get; set; }
        public string Description { get; set; }
    }

    public class AvailableReward : BaseModel
    {
        public decimal AvailableRewardMoney { get; set; }
    }
    public class RewardEarnProductDto
    {
        public AvailableReward AvailableReward { get; set; }
        public List<RewardProductDto> ProductDtos { get; set; }
    }

    public class EarnedRewardProducts : BaseModel
    {
        public decimal Redeem { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public short DeliveryStatus { get; set; }
    }

    public class RebateProgressDto {
        public decimal CurrentMonth_OrderAmount { get; set; }
        public decimal TierProgress_Percent { get; set; }
        public int CurrentTier { get; set; }
        public decimal RebatePoint { get; set; }
        public decimal Need { get; set; }
        public int RemainingDays { get; set; }

    }   
}
