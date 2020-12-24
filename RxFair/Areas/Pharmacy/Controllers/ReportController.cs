using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.ExcelUtility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class ReportController : BaseController<ReportController>
    {
        private readonly IReportService _report;
        private readonly IOrderService _order;
        private readonly IDistributorOrderChargeService _orderCharge;
        private readonly IExcelReports _excelReports;

        public ReportController(IDistributorOrderChargeService orderCharge,IReportService report, IOrderService order, IExcelReports excelReports)
        {
            _orderCharge = orderCharge;
            _report = report;
            _order = order;
            _excelReports = excelReports;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PharmacyOrderSummary()
        {
            BindDropdownList();
            return View();
        }

        public IActionResult UnshippedOrders()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyOrderSummary(JQueryDataTableParamModel param, string orderId, string startDate, string endDate)
        {
            try
            {
                var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(2, new SqlParameter("@OrderId", SqlDbType.VarChar) { Value = orderId ?? "" });
                parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });
                parameters.Insert(5, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId});
                var allList = await _report.GetPharmacyOrderSummary(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetPharmacyOrderSummary");
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
        public async Task<IActionResult> GetUnshippedOrders(JQueryDataTableParamModel param, string startDate, string endDate)
        {
            try
            {
                var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });
                parameters.Insert(5, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
                var allList = await _report.GetUnshippedOrders(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetUnshippedOrders");
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

        #region Export To Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcelPharmacyOrderSummary(string orderId, string startDate, string endDate,string search)
        {
            var PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            var memory = await _excelReports.PharmacyOrderSummary(GlobalRxFair.Value.CurrentTimeZoneId, orderId, startDate, endDate,search, PharmacyId);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PharmacyOrderSummary.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcelUnshippedOrders(string startDate, string endDate, string search)
        {
            var memory = await _excelReports.UnshippedOrders(GlobalRxFair.Value.CurrentTimeZoneId, startDate, endDate, search);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UnshippedOrders.xlsx");
        }
        #endregion

        #region Controller Common
        private void BindDropdownList()
        {
            var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            var pharmacyMatch = _order.GetAll().Where(x => x.PharmacyId == pharmacyId).Select(x=>x.Id).ToList();
             
            var statusMatch = _orderCharge.GetAll().Where(x=>(x.OrderStatus== (short)GlobalEnums.OrderStatus.Confirmed || x.OrderStatus==(short)GlobalEnums.OrderStatus.Pending)).Select(x=>x.OrderId).ToList();
            var UniqueOrderList= statusMatch.Where(a => pharmacyMatch.Any(b => b.Equals(a))).ToList();
            List<string> temp = new List<string>();
            
            foreach (var item in UniqueOrderList)
            {
                var data = _order.GetSingle(x => x.Id == item).UniqueOrder;
                temp.Add(data);
            }
            ViewBag.OrderIdList = temp.Select(x => new SelectListItem { Text = x.ToString(), Value = x.ToString(), }).OrderBy(x => x.Text).ToList();
        }
    #endregion
}
}