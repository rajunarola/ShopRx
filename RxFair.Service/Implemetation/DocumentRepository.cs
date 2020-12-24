using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Data.Extensions;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Implemetation.BaseService;
using RxFair.Service.Interface;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RxFair.Service.Implemetation
{
    public class DocumentRepository : GenericRepository<DocumentMaster>, IDocumentService
    {
        private readonly RxFairDbContext _context;
        public DocumentRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DocumentView>> GetDocumentList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetDocumentList, paraObjects);
            return Common.ConvertDataTable<DocumentView>(dataSet.Tables[0]);
        }
    }

    public class DistributorDocumentRepository : GenericRepository<DistributorDocumentMaster>, IDistributorDocumentService
    {
        private readonly RxFairDbContext _context;
        public DistributorDocumentRepository(RxFairDbContext context) : base(context)
        {
            _context = context;
        }

    }
}