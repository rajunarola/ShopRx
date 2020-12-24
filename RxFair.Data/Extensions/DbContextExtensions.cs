using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RxFair.Data.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<DataSet> GetQueryDatatableAsync(this Microsoft.EntityFrameworkCore.DbContext context, string sqlQuery, SqlParameter[] sqlParam = null, CommandType type = CommandType.StoredProcedure)
        {
            using (DbCommand cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.Connection = context.Database.GetDbConnection();
                cmd.CommandType = type;
                cmd.CommandTimeout = 0;
                //ctx.Database.SetCommandTimeout(3600);
                if (sqlParam != null)
                { cmd.Parameters.AddRange(sqlParam); }
                cmd.CommandText = sqlQuery;
                using (DbDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    dataAdapter.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    await Task.Run(() => dataAdapter.Fill(ds));
                    return ds;
                }
            }
        }

        public static DataSet GetQueryDatatable(this Microsoft.EntityFrameworkCore.DbContext context, string sqlQuery, SqlParameter[] sqlParam = null, CommandType type = CommandType.StoredProcedure)
        {
            using (DbCommand cmd = context.Database.GetDbConnection().CreateCommand())
            {
                cmd.Connection = context.Database.GetDbConnection();
                cmd.CommandType = type;
                cmd.CommandTimeout = 0;
                if (sqlParam != null)
                { cmd.Parameters.AddRange(sqlParam); }
                cmd.CommandText = sqlQuery;
                using (DbDataAdapter dataAdapter = new SqlDataAdapter())
                {
                    dataAdapter.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    dataAdapter.Fill(ds);
                    return ds;
                }
            }
        }
    }
}
