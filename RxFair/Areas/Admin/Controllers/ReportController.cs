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
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.ExcelUtility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize, Area("Admin")]
    public class ReportController : BaseController<ReportController>
    {
        private readonly IExcelReports _excelReports;
        private readonly IReportService _report;
        private readonly IPharmacyService _pharmacy;
        private readonly IDistributorService _distributor;
        private readonly IDistributorOrderChargeService _distributorOrderCharge;

        public ReportController(IReportService report, IPharmacyService pharmacy, IDistributorService distributor, IDistributorOrderChargeService distributorOrderCharge, IExcelReports excelReports)
        {
            _report = report;
            _pharmacy = pharmacy;
            _distributor = distributor;
            _distributorOrderCharge = distributorOrderCharge;
            _excelReports = excelReports;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PharmacyPurchaseOrder()
        {
            BindDropdownList();
            return View();
        }

        public IActionResult DistributorSalesOrder()
        {
            BindDropdownList();
            return View();
        }

        public IActionResult ReviewMedicinePrice()
        {
            BindDropdownList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyPurchaseOrder(JQueryDataTableParamModel param, long pharmacyId, string startDate, string endDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(2, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
                parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                var allList = await _report.GetPharmacyPurchaseOrder(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetPharmacyPurchaseOrder");
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
        public async Task<IActionResult> GetReviewMedicinePrice(List<string> distributorList, JQueryDataTableParamModel param)
        {
            ReportDto model = new ReportDto();
            distributorList = distributorList ?? new List<string>();
            model.Distributor = string.Join(",", distributorList ?? new List<string>());
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(1, new SqlParameter("@DistributorId", SqlDbType.VarChar) { Value = model.Distributor ?? "" });
                var allList = await _report.GetReviewMedicinePrice(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetReviewMedicinePrice");
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
        public async Task<IActionResult> GetReviewMedicineDistributorWise(long id, JQueryDataTableParamModel param)
        {
            try
            {

                //var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                //var parameters = new List<SqlParameter>
                //{
                //    new SqlParameter("@MedicineId",SqlDbType.Int){Value = id},
                //    new SqlParameter("@TimeZoneId",SqlDbType.Int){Value = GlobalRxFair.Value.CurrentTimeZoneId},
                //    //new SqlParameter("@Search",SqlDbType.VarChar){Value = search ?? ""},

                //   searchRecords
                //};

                //var allList = await _report.GetReviewMedicineDistributorWise(parameters.ToArray());

                //return JsonResponse.GenerateJsonResult(1, "", allList);


                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(1, new SqlParameter("@MedicineId", SqlDbType.Int) { Value = id });
                var allList = await _report.GetReviewMedicineDistributorWise(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetReviewMedicineDistributorWise");
                return JsonResponse.GenerateJsonResult(0, "");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyPurchaseDistributorOrder(long id, string startDate, string endDate, string search, JQueryDataTableParamModel param)
        {
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(1, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = id });
                parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });

                var allList = await _report.GetPharmacyPurchaseDistributorOrder(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetPharmacyPurchaseDistributorOrder");
                return JsonResponse.GenerateJsonResult(0, "");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributorSalesOrder(JQueryDataTableParamModel param, long distributorId, string startDate, string endDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(startDate))
                    startDate = Convert.ToDateTime(startDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(endDate))
                    endDate = Convert.ToDateTime(endDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(1, new SqlParameter("@IsExcel", SqlDbType.Bit) { Value = false });
                parameters.Insert(2, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                parameters.Insert(3, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = startDate ?? "" });
                parameters.Insert(4, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = endDate ?? "" });
                var allList = await _report.GetDistributorSalesOrder(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetDistributorSalesOrder");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult ViewMedicines(long id, string startDate, string endDate)
        {
         
            var model = new ReportDto
            {
                OrderChargeId = id,
                startDate = Convert.ToDateTime(startDate).ToShortDateString(),
                endDate = endDate==null?"":Convert.ToDateTime(endDate).ToShortDateString()
                
            };
            if (model.endDate == "01-01-0001" || model.endDate==null)
                model.endDate = "";

            return View(@"Components/_ViewMedicines", model);
        }

        #region Export To Excel
        [HttpGet]
        public async Task<IActionResult> ExportToExcelPharmacyPurchaseOrder(long pharmacyId, string startDate, string endDate, string search)
        {
            var memory = await _excelReports.AdminPharmacyPurchaseOrder(GlobalRxFair.Value.CurrentTimeZoneId, pharmacyId, startDate, endDate, search);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PharmacyPurchaseDistributorOrder.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcelDistributorSalesOrder(long distributorId, string startDate, string endDate, string search)
        {
            var memory = await _excelReports.AdminDistributorSalesOrder(GlobalRxFair.Value.CurrentTimeZoneId, distributorId, startDate, endDate, search);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DistributorSalesOrder.xlsx");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcelReviewMedicinePrice(List<string> distributorList, string search)
        {
            var memory = await _excelReports.ReviewMedicinePrice(GlobalRxFair.Value.CurrentTimeZoneId, distributorList, search);

            return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReviewMedicinePrice.xlsx");
        }
        #endregion

        public IActionResult ViewDistributorDetails(long id)
        {
            var model = new ReportDto
            {
                MedicineId = id
            };

            return View(@"Components/_ViewDistributorDetails", model);
        }

        #region Controller Common
        private void BindDropdownList()
        {
            ViewBag.PharmacyList = _pharmacy.GetAll().Select(x => new SelectListItem { Text = x.PharmacyAdminUser.Pharmacy.PharmacyName, Value = x.Id.ToString(), }).OrderBy(x => x.Text).ToList();
            ViewBag.DistributorList = _distributorOrderCharge.GetAll().DistinctBy(x => x.DistributorId).Select(x => new SelectListItem { Text = x.Distributor.CompanyName, Value = x.DistributorId.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}