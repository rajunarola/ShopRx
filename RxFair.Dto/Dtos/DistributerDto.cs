using System;
using System.Collections.Generic;

namespace RxFair.Dto.Dtos
{
    public class DistributorDto : NewDistributorRequestDto
    {
        //personalInfo

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string FaxNumber { get; set; }

        //Subscriptions info
        public DistributorSubscriptionDto SubscriptionDto { get; set; }

        //OrderSettingInfo
        public DistributerOrderSettingDto DistributerOrderSettingDto { get; set; }
    }

    public class DistributorSubscriptionDto : BaseModel
    {
        public long DistributorId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string SubscriptionTypeName { get; set; }
        public string DateStart { get; set; }
        public DateTime StartDate { get; set; }
        public string DateEnd { get; set; }
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public long SubscriptionTypeId { get; set; }
        public long IsExpire { get; set; }
        public List<SubscriptionTypeDto> SubscriptionTypeDtos { get; set; }
    }

    public class DistributorSubscriptionHistoryDto : BaseModel
    {
        public long DistributorId { get; set; }
        public string SubscriptionTypeName { get; set; }
        public string DateStart { get; set; }
        public DateTime StartDate { get; set; }
        public string DateEnd { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class DistributerOrderSettingDto : BaseModel
    {
        public long DistributorId { get; set; }
        public long TimeZoneId { get; set; }
        public decimal MinOrderAmount { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal OverNightAmount { get; set; }
        public bool ServiceDayMonday { get; set; }
        public bool ServiceDayTuesday { get; set; }
        public bool ServiceDayWednesday { get; set; }
        public bool ServiceDayThursday { get; set; }
        public bool ServiceDayFriday { get; set; }
        public bool ServiceDaySaturday { get; set; }
        public bool ServiceDaySunday { get; set; }
        public string MondayCutOffTime { get; set; }
        public string TuesdayCutOffTime { get; set; }
        public string WednesdayCutOffTime { get; set; }
        public string ThursdayCutOffTime { get; set; }
        public string FridayCutOffTime { get; set; }
        public string SaturdayCutOffTime { get; set; }
        public string SundayCutOffTime { get; set; }
    }

    public class DistributorProfileDto : NewDistributorRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string FaxNumber { get; set; }
        public bool SameAsCompany { get; set; }
        public string UserProfileImage { get; set; }
        public DistributerOrderSettingDto DistributerOrderSettingDto { get; set; }
    }
}
