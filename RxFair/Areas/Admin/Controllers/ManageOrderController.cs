using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize, Area("Admin")]
    public class ManageOrderController : BaseController<ManageOrderController>
    {
        private readonly IDistributorOrderChargeService _distributorOrderCharge;
        private readonly IDistributorOrderService _distributorOrder;
        private readonly IUserService _user;
        private readonly IOrderService _order;
        public ManageOrderController(IDistributorOrderChargeService distributorOrderCharge, IOrderService order, IUserService user, IDistributorOrderService distributorOrder)
        {
            _distributorOrderCharge = distributorOrderCharge;
            _distributorOrder = distributorOrder;
            _order = order;
            _user = user;

        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _distributorOrderCharge.GetOrders(parameters.ToArray());
                //var total = allList.Count;
                allList.ForEach(x =>
                {
                    // x.InvoiceFile = $@"{FilePathList.Invoice}\{x.InvoiceFile}";
                });
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
                ErrorLog.AddErrorLog(ex, "GetOrders");
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
        public IActionResult ViewOrderDetail(int id)
        {
            var result = _distributorOrderCharge.GetSingle(x => x.Id == id);

            var model = new ViewOrderDetailDto
            {
                Id = result.OrderId,
                DateCreated = result.Order.CreatedDate.ToDefaultDateTime(GlobalFormates.DefaultDate),
                OrderBy = result.Order.OrderCreatedBy.FullName,
                OrderDate = result.Order.OrderDate.ToDefaultDateTime(GlobalFormates.DefaultDate),
                Pharmacy = result.Order.Pharmacy.PharmacyName,
                DistributorId = result.DistributorId,
                Distributor = result.Distributor.CompanyName,
                OrderNo = result.Order.UniqueOrder,
                GrandTotal = result.GrandTotal,
                OrderStatus = ((OrderStatus)result.OrderStatus).ToString(),
                PaymentDate = result.PaymentDate.ToDefaultDateTime(GlobalFormates.DefaultDate),
                PaymentNote = result.PaymentNote,
                PaymentAmount = result.PaymentAmount ?? 0,
                ShippingDate = result.ShippingDate.ToDefaultDateTime(GlobalFormates.DefaultDate),
                ShippingCost = result.ShippingTotal,
                OvernightCost = result.OverNightCharge ?? 0,
                TrackingNumber = result.TrackingNo ?? "",
                TrackingLink = result.TrackingLink ?? ""
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersDetails(int orderChargeId, int distributorId, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@OrderChargeId", SqlDbType.BigInt) { Value = orderChargeId });
                parameters.Insert(1, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                var allList = await _distributorOrder.GetOrdersDetails(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetOrdersDetails");
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
        [Route("/Admin/ManageOrder/DownloadInvoice/{orderChargeId}/{uniqueId}")]
        public IActionResult DownloadInvoice(long orderChargeId, string uniqueId)
        {
            try
            {
                var orderObj = _order.GetSingle(x => x.UniqueOrder == uniqueId);
                var orderChargeObj = _distributorOrderCharge.GetSingle(x => x.Id == orderChargeId);
                var distributor = orderChargeObj.Distributor.CompanyName;
                var pharmacy = _order.GetSingle(x => x.UniqueOrder == uniqueId).Pharmacy.PharmacyAdminUser.FullName;
                var medicineCount = orderChargeObj.DistributorOrders.Select(x => x.MedicineId).Count();
                InvoiceDto invoiceDto = new InvoiceDto
                {
                    UniqueOrder = orderObj.UniqueOrder,
                    Pharmacy = pharmacy,
                    Distributor = distributor,
                    OrderDate = orderObj.OrderDate.ToShortDateString(),
                    OrderDueDate = orderObj.OrderDate.AddDays(30).ToShortDateString(),
                    GrandTotal = orderChargeObj.GrandTotal,
                    TotalPrice = orderChargeObj.SubTotal,
                    MedicineCount = medicineCount,
                    OvernightCost = orderChargeObj.OverNightCharge ?? 0,
                    ShippingCost = orderChargeObj.ShippingTotal
                };
                var medicineList = new List<MedicineList>();
                foreach (var medinfo in orderChargeObj.DistributorOrders)
                {
                    MedicineList tempMedicine = new MedicineList()
                    {
                        Quantity = medinfo.Quantity,
                        MedicineId = medinfo.MedicineId,
                        MedicineName = medinfo.MedicineMaster.DrugName,
                        MedicinePrice = medinfo.Price
                    };
                    medicineList.Add(tempMedicine);
                }
                invoiceDto.MedicineList = new List<MedicineList>();
                if (medicineList.Any())
                {
                    invoiceDto.MedicineList = medicineList;
                }
             //  return View("DownloadInvoice", invoiceDto);
               return new ViewAsPdf("DownloadInvoice", invoiceDto) { FileName = invoiceDto.UniqueOrder + ".pdf" };
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get/DownloadInvoice");
                return View("Error", "Home");
            }
        }
    }
}