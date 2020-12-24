using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
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
    public class ContactRequestRepository : GenericRepository<ContactRequest>, IContactRequestService
    {
        private readonly RxFairDbContext _context;
        public ContactRequestRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ContactRequestView>> GetContactRequestListAsync(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetContactRequestList, paraObjects);
            return Common.ConvertDataTable<ContactRequestView>(dataSet.Tables[0]);
        }
    }
}

