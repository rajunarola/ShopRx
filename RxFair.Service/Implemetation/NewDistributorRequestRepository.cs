using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;

namespace RxFair.Service.Implemetation
{
    public class NewDistributorRequestRepository : GenericRepository<NewDistributorRequest>, INewDistributorRequestService
    {
        private readonly RxFairDbContext _context;
        public NewDistributorRequestRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> AddDistributer(long id)
        {
            var objDistributer = _context.NewDistributorRequest.FirstOrDefault(x => x.Id == id);
            Distributor AccptedDistributer = new Distributor
            {
                Address = objDistributer.Address,
                City = objDistributer.City,
                Email = objDistributer.Email,
                Mobile = objDistributer.Mobile,
                Phone = objDistributer.Phone,
                StateId = objDistributer.StateId,
                ZipCode = objDistributer.ZipCode,
                IsActive = objDistributer.IsActive,
                Id = objDistributer.Id,
                CompanyName = objDistributer.CompanyName,
                ContactAddress = objDistributer.ContactAddress,
                ContactEmail = objDistributer.ContactEmail,
                ContactMobile = objDistributer.ContactMobile,
                ContactName = objDistributer.ContactName,

            };
            var result = await _context.Distributor.AddAsync(AccptedDistributer);
            if (result == null) return 0;
            else
                return 1;
        }

        public async Task<List<NewDistributorRequestDto>> GetNewDistributorRequestList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetNewDistributorRequestList, paraObjects);
            return Common.ConvertDataTable<NewDistributorRequestDto>(dataSet.Tables[0]);
        }


    }

}

