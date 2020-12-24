using RxFair.Dto.Enum;
using System;

namespace RxFair.Utility.Extension
{
    public static class DateExtension
    {
        /// <summary>
        /// Convert given date time value to UTC Timezone from given user Timezone
        /// </summary>
        /// <param name="value">Datetime to convert</param>
        /// <param name="userTimeZone"></param>
        /// <returns></returns>
        public static DateTime UserTimeToUtc(this DateTime value, string userTimeZone)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            var date = TimeZoneInfo.ConvertTimeToUtc(value, timezone);
            var dateWithOutTimeZone = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            return dateWithOutTimeZone;
        }

        /// <summary>
        /// Convert given date time value to User Timezone from Utc Timezone
        /// </summary>
        /// <param name="value"></param>
        /// <param name="userTimeZone"></param>
        /// <returns></returns>
        public static DateTime UtcToUserTime(this DateTime value, string userTimeZone)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            var date = TimeZoneInfo.ConvertTimeFromUtc(value, timezone);
            var dateWithOutTimeZone = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            return dateWithOutTimeZone;
        }

        public static DateTime UtcToUserTime(this DateTime? value, string userTimeZone)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            var date = TimeZoneInfo.ConvertTimeFromUtc(value.Value, timezone);
            var dateWithOutTimeZone = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            return dateWithOutTimeZone;
        }


        public static string ToDefaultTime(this TimeSpan? time, string format = "")
        {
            if (!time.HasValue) return "";
            format = string.IsNullOrEmpty(format) ? "hh:mm tt" : format;
            return DateTime.Today.Add(time.Value).ToString(format);
        }

        public static string ToDefaultDateTime(this DateTime? dateTime, string format = "")
        {
            if (!dateTime.HasValue) return "";
            format = string.IsNullOrEmpty(format) ? GlobalFormates.FullDate : format;
            return dateTime.Value.ToString(format);
        }
        public static string ToDefaultDateTime(this DateTime dateTime, string format = "")
        {
            format = string.IsNullOrEmpty(format) ? GlobalFormates.FullDate : format;
            return dateTime.ToString(format);
        }

        public static DateTime DistributorToPharmacyUserTime(this TimeSpan? value, string userTimeZone)
        {
            var pharmacyUserDateTime = DateTime.UtcNow.Date.Add(value.Value);
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            var currentTimezone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
            var distributorDateTime = TimeZoneInfo.ConvertTimeFromUtc(pharmacyUserDateTime, timezone);
            //Converting Distributor Time into UTC Time.    
            var newdistributorDateTime = TimeZoneInfo.ConvertTimeToUtc(distributorDateTime);
            //converting UTC Time to Local Time(Pharmacy Time).
            var PharmacyTime = TimeZoneInfo.ConvertTimeFromUtc(distributorDateTime, currentTimezone);
            //var dateWithOutTimeZone = DateTime.SpecifyKind(distributorDateTime, DateTimeKind.Unspecified);
            return PharmacyTime;
        }
    }
}
