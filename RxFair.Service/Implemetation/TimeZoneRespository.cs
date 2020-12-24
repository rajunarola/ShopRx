using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Service.Implemetation
{
   public class TimeZoneRespository : GenericRepository<TimeZoneMaster>, ITimeZoneService
    {
        private readonly RxFairDbContext _context;
        public TimeZoneRespository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
        
    }
}
