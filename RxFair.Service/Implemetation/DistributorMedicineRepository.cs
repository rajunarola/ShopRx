using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RxFair.Service.Implemetation
{
    public class DistributorMedicineRepository : GenericRepository<DistributorMedicine>, IDistributorMedicineService
    {
        private readonly RxFairDbContext _context;
        public DistributorMedicineRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }
    }
}