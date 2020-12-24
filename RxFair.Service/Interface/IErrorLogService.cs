using System;
using RxFair.Data.DbModel;
using RxFair.Service.Interface.BaseInterface;

namespace RxFair.Service.Interface
{

    public interface IErrorLogService : IGenericService<ErrorLog>
    {
        void AddErrorLog(Exception ex, string appType);
    }
}
