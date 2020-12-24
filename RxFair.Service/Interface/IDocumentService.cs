using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface.BaseInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RxFair.Service.Interface
{
    public interface IDocumentService : IGenericService<DocumentMaster>
    {
        Task<List<DocumentView>> GetDocumentList(SqlParameter[] paraObjects);
    }

    public interface IDistributorDocumentService : IGenericService<DistributorDocumentMaster>
    {
    }
}