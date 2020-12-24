using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.ExcelUtility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class ReportController : BaseController<ReportController>
    {
        #region Fields
        private readonly IReportService _report;
        private readonly IExcelReports _excelReports;
        private readonly ICommissionHistoryService _commissionHistory;

        #endregion

        #region Ctor
        public ReportController(IReportService report, ICommissionHistoryService commissionHistory, IExcelReports excelReports)
        {
            _report = report;
            _commissionHistory = commissionHistory;
            _excelReports = excelReports;
        }
        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SalesOrderReport()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetSalesOrderReport(JQueryDataTableParamModel param, string startDate, string endDate)
        {
            try
            {
                var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });
                parameters.Insert(4, new SqlParameter("@DistributorId", SqlDbType.Int) { Value = distributorId });
                var allList = await _report.GetSalesOrderReport(parameters.ToArray());
                //var total = allList.Count;
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetSalesOrderReport");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyPurchaseDistributorOrder(long id, string startDate, string endDate, string search)
        {
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@TimeZoneId",SqlDbType.Int){Value = GlobalRxFair.Value.CurrentTimeZoneId},
                    new SqlParameter("@IsExcel",SqlDbType.Bit){Value = false},
                    new SqlParameter("@OrderChargeId",SqlDbType.Int){Value = id},
                    new SqlParameter("@StartDate",SqlDbType.VarChar){Value =startDate ?? ""},
                    new SqlParameter("@EndDate",SqlDbType.VarChar){Value = endDate ?? ""},
                   searchRecords
                };

                var allList = await _report.GetPharmacyPurchaseDistributorOrder(parameters.ToArray());

                return JsonResponse.GenerateJsonResult(1, "", allList);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetPharmacyPurchaseDistributorOrder");
                return JsonResponse.GenerateJsonResult(0, "");
            }
        }

        public IActionResult CommissionInvoices()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributorCommissionInvoiceList(JQueryDataTableParamModel param)
        {
            try
            {
                var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                var allList = await _commissionHistory.GetDistributorCommissionInvoiceList(parameters.ToArray());
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetDistributorCommissionInvoiceList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }


        #region Export To Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcelSalesOrderReport(string startDate, string endDate, string search)
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            var memory = await _excelReports.SalesOrderReport(GlobalRxFair.Value.CurrentTimeZoneId, distributorId, startDate, endDate, search);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SalesOrderReport.xlsx");
        }

        #endregion
        #endregion

        #region Common
        #endregion

        #region Controller Common
        #endregion
    }
}