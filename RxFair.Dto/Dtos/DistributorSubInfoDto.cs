using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Dto.Dtos
{
  public  class DistributorSubInfoDto
    {
        public long Id { get; set; }
        public string SubscriptionTypeName { get; set; }
        public string CompanyName { get; set; }
        public int PendingDays { get; set; }
    }
}
