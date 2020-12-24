using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace RxFair.StandardLibrary
{
    public class WatchList
    {
        public string NDC { get; set; }
        public string MedicineName { get; set; }
        public string CompanyName { get; set; }
        public int Quantity { get; set; }
        public float? WacpackagePrice { get; set; }
        public float MatchPrice { get; set; }
        public DateTime MedicineDate { get; set; }
        public long PharmacyId { get; set; }
        public long Id { get; set; }
        public string PharmacyName { get; set; }
        public string Email { get; set; }
        public bool IsNotified { get; set; }
    }

    public class DealOfTheDay
    {
        public string NDC { get; set; }
        public string CompanyName { get; set; }
        public string MedicineName { get; set; }
        public short DealType { get; set; }
        public DateTime? DealDate { get; set; }
        public float DealPrice { get; set; }
    }

    public class DealOfTheDayPharmacy
    {
        public long PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string Email { get; set; }
    }

    public class PharmacyOrder
    {
        public long PharmacyId { get; set; }
        public long OrderId { get; set; }
        public decimal SubTotal { get; set; }
        public int LastOrderDays { get; set; }
    }

    public class RewardMoneyMaster 
    {
        public long RewardTypeId { get; set; }
        public string Description { get; set; }
        public decimal MinRange { get; set; }
        public decimal MaxRange { get; set; }
        public decimal Referral { get; set; }
    }

}
