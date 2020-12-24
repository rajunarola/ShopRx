using RxFair.Dto.Dtos;
using System.Collections.Generic;

namespace RxFair.Dto.Global
{
    public class GlobalRxFair
    {
        public long CurrentTimeZoneId { get; set; }
        public string CurrentTimeZone { get; set; }
        public string PhysicalUrl { get; set; }
        public bool IsDaylightSavingTime { get; set; }
    }
}
