using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace RxFair.Models
{
    public class JQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        //public string sSearch { get; set; }


        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Sort column index
        /// </summary>
        [FromQuery]
        public int iSortCol_0 { get; set; }

        /// <summary>
        /// Sort Column Direction
        /// </summary>
        [FromQuery]
        public string sSortDir_0 { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public List<CustomSearch> CustomSearchs { get; set; }

    }

    public class JQueryDataTableSqlParam
    {
        public SqlParameter SearchRecord { get; set; }
        public List<SqlParameter> Parameters { get; set; }
    }
    public class CustomSearch
    {
        public string SearchValue { get; set; }
        public string SearchColumnName { get; set; }
    }
}