using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using RxFair.Utility.Helpers;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class ManageOrderController : BaseController<ManageOrderController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IOrderService _order;
        private readonly IDistributorOrderChargeService _orderCharge;
        private readonly IDistributorOrderService _distributorOrder;
        private readonly EmailService _emailService;
        #endregion

        #region Ctor
        public ManageOrderController(IOrderService order, IOptions<EmailSettingsGmail> emailSettingsGmail, IDistributorService distributor, IDistributorOrderService distributorOrder, IDistributorOrderChargeService orderCharge)
        {
            _distributor = distributor;
            _order = order;
            _emailService = new EmailService(emailSettingsGmail);
            _distributorOrder = distributorOrder;
            _orderCharge = orderCharge;
        }
        #endregion

        #region Methods
        public IActionResult ManageOrders()
        {
            BindDropDownList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingOrderList(JQueryDataTableParamModel param)
        {
            try
            {

                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = DistributorId });
                var allList = await _orderCharge.GetPendingOrderList(parameters.Parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "SearchMedicineList");
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
        public async Task<IActionResult> GetAllOrderList(JQueryDataTableParamModel param, short orderStatus, string fromDate, string toDate)
        {
            try
            {
                if (!string.IsNullOrEmpty(fromDate))
                    fromDate = Convert.ToDateTime(fromDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(toDate))
                    toDate = Convert.ToDateTime(toDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = DistributorId });
                parameters.Parameters.Insert(1, new SqlParameter("@OrdreStatus", SqlDbType.SmallInt) { Value = orderStatus });
                parameters.Parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = fromDate ?? "" });
                parameters.Parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = toDate ?? "" });
                var allList = await _orderCharge.GetAllOrderList(parameters.Parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "SearchMedicineList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageOrderStatus(long id, OrderStatus status)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _orderCharge.GetSingle(x => x.Id == id);
                    result.OrderStatus = (short)status;
                    await _orderCharge.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    if (result.OrderStatus != (short)OrderStatus.Pending)
                        await SendUpdateOrderEmail(result.Id);
                    return JsonResponse.GenerateJsonResult(1, $@"Order Status has been {(status==GlobalEnums.OrderStatus.Confirmed?"confirmed":"cancelled")}." , result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageOrderStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult ViewOrderDetails(string id)
        {
            try
            {
                //BindDropDownList();
                var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var orderDetails = _order.GetSingle(x => x.UniqueOrder == id);
                var orderChargeDetails = orderDetails.OrderCharges.FirstOrDefault(x => x.OrderId == orderDetails.Id && x.DistributorId == distributorId);
                GetOrderDetailList orderDetail = new GetOrderDetailList();
                orderDetail.UniqueOrder = orderDetails.UniqueOrder;

                //Order Details
                //orderDetail.OrderDate = orderDetails.OrderDate.ToShortDateString();
                orderDetail.OrderDate = Convert.ToDateTime(orderDetails.OrderDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                orderDetail.Pharmacy = orderDetails.Pharmacy.PharmacyName;
                orderDetail.PharmacyId = orderDetails.PharmacyId;
                orderDetail.Distributor = orderChargeDetails.Distributor.CompanyName;
                orderDetail.GrandTotal = orderChargeDetails.GrandTotal;
                orderDetail.PaymentAmount = orderChargeDetails.PaymentAmount ?? 0;
                orderDetail.PaymentDate = orderChargeDetails.PaymentDate == null ? "" : Convert.ToDateTime(orderChargeDetails.PaymentDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                //orderDetail.PaymentDate = orderChargeDetails.PaymentDate == null ? "" : Convert.ToDateTime(orderChargeDetails.PaymentDate).ToShortDateString();
                orderDetail.PaymentNote = orderChargeDetails.PaymentNote ?? "";
                orderDetail.TrackingLink = orderChargeDetails.TrackingLink ?? "";
                orderDetail.TrackingNumber = orderChargeDetails.TrackingNo ?? "";
                orderDetail.ShippingDate = orderChargeDetails.ShippingDate == null ? "" : Convert.ToDateTime(orderChargeDetails.ShippingDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                //orderDetail.ShippingDate = orderChargeDetails.ShippingDate == null ? "" : Convert.ToDateTime(orderChargeDetails.ShippingDate).ToShortDateString();
                orderDetail.OvernightCost = orderChargeDetails.OverNightCharge == null ? 0 : (decimal)orderChargeDetails.OverNightCharge;
                
                //Order Status Details
                //OrderStatus = orderDetails.OrderStatus,

                //Shipping Details    
                orderDetail.ShippingCost = orderChargeDetails.ShippingTotal;
                orderDetail.orderStatus = orderChargeDetails.OrderStatus;
                orderDetail.OrderStatus = Enum.GetName(typeof(GlobalEnums.OrderStatus), orderChargeDetails.OrderStatus);
                GanerateOrderStatusList(orderDetail.orderStatus);
                //var OrderStatusList = EnumHelpers.EnumToList<GlobalEnums.OrderStatus>().Select(x => new SelectListItem
                //{
                //    Text = x.Name,
                //    Value = x.Value.ToString()
                //}).OrderBy(x => x.Text).ToList();

                //switch (orderDetail.orderStatus)
                //{
                //    case (short)OrderStatus.Confirmed: ViewBag.OrderStatus = OrderStatusList.GetRange(2, 4); break;
                //    case (short)OrderStatus.Shipped: ViewBag.OrderStatus = OrderStatusList.GetRange(3, 3); break;
                //    case (short)OrderStatus.Delivered: ViewBag.OrderStatus = OrderStatusList.GetRange(4, 1); break;
                //    default: ViewBag.OrderStatus = OrderStatusList;break;
                //}
                return View(orderDetail);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post/ViewOrderDetails");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributorOrderChargeMedicineList(JQueryDataTableParamModel param, string uniqueOrderId, long PharmacyId)
        {
            try
            {
                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = DistributorId });
                parameters.Parameters.Insert(1, new SqlParameter("@UniqueOrderId", SqlDbType.VarChar) { Value = uniqueOrderId });
                parameters.Parameters.Insert(2, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = PharmacyId });
                var allList = await _orderCharge.GetDistributorOrderChargeMedList(parameters.Parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetDistributorOrderChargeMedicineList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }


        public IActionResult UpdateMedicineQty(long id)
        {
            ViewBag.currentQty = _distributorOrder.GetById(id).Quantity;
            ViewBag.distributorOrderId = id;
            return View(@"Components/_updateMedicineQty");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMedicineQty(long id, int qty)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    var distributor = _distributor.GetById(distributorId);
                    var distributorOrder = _distributorOrder.GetById(id);
                    distributorOrder.Quantity = qty;
                    var currentMedPrice = distributor.MedicinePriceMasters.FirstOrDefault(x => x.DistributorId == distributorId && x.MedicineId == distributorOrder.MedicineId).WacpackagePrice;
                    distributorOrder.TotalPrice = 0;
                    distributorOrder.TotalPrice = Convert.ToDecimal(distributorOrder.Quantity * currentMedPrice);
                    var dOrder = await _distributorOrder.UpdateAsync(distributorOrder, Accessor, User.GetUserId());

                    var distributorOrderCharge = _orderCharge.GetById(dOrder.OrderChargeId);
                    distributorOrderCharge.SubTotal = 0;
                    distributorOrderCharge.GrandTotal = 0;
                    foreach (var item in distributorOrderCharge.DistributorOrders)
                    {
                        distributorOrderCharge.SubTotal += item.Quantity * item.Price;
                    }
                    distributorOrderCharge.GrandTotal = (distributorOrderCharge.SubTotal + distributorOrderCharge.ShippingTotal + (distributorOrderCharge.OverNightCharge == null ? 0 : (decimal)distributorOrderCharge.OverNightCharge));
                    var orderCharge = await _orderCharge.UpdateAsync(distributorOrderCharge, Accessor, User.GetUserId());
                    var order = _order.GetById(orderCharge.OrderId);
                    order.OrderSubTotal = 0;
                    order.OrderGrandTotal = 0;
                    foreach (var item in order.OrderCharges)
                    {
                        order.OrderSubTotal += item.SubTotal;
                    }

                    order.OrderGrandTotal = order.OrderSubTotal + order.OrderShippingTotal + (distributorOrderCharge.OverNightCharge == null ? 0 : (decimal)distributorOrderCharge.OverNightCharge);

                    await _order.UpdateAsync(order, Accessor, User.GetUserId());

                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Quantity updated successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/UpdateMedicineQty");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(GetOrderDetailList model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    var currentOrder = await _order.GetSingleAsync(x => x.UniqueOrder == model.UniqueOrder);
                    var ordercharge = _orderCharge.GetSingle(x => x.DistributorId == distributorId && x.OrderId == currentOrder.Id);

                    ordercharge.PaymentAmount = model.PaymentAmount;
                    ordercharge.PaymentDate = Convert.ToDateTime(model.PaymentDate);
                    ordercharge.ShippingDate = Convert.ToDateTime(model.ShippingDate);
                    ordercharge.PaymentNote = model.PaymentNote;    
                    ordercharge.TrackingNo = model.TrackingNumber;
                    ordercharge.TrackingLink = model.TrackingLink;
                    ordercharge.OrderStatus = (short)EnumHelpers.GetValueFromDescription<OrderStatus>(model.OrderStatus);

                    await _orderCharge.UpdateAsync(ordercharge, Accessor, User.GetUserId());
                    txscope.Complete();
                    if (ordercharge.OrderStatus != (short)OrderStatus.Pending)
                        await SendUpdateOrderEmail(ordercharge.Id);
                    return JsonResponse.GenerateJsonResult(1, "Your order details has been updated successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/UpdateOrder");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult DownloadInvoice(string id)
        {
            try
            {
                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var orderObj = _order.GetSingle(x => x.UniqueOrder == id);

                var orderChargeObj = _orderCharge.GetSingle(x => x.OrderId == orderObj.Id && x.DistributorId == DistributorId);
                var medicineCount = orderChargeObj.DistributorOrders.Select(x => x.MedicineId).Count();
                InvoiceDto InvoiceDto = new InvoiceDto
                {
                    UniqueOrder = orderObj.UniqueOrder,
                    GrandTotal = orderChargeObj.GrandTotal,
                    OvernightCost = orderChargeObj.OverNightCharge ?? 0,
                    TotalPrice = orderChargeObj.SubTotal,
                    Pharmacy = orderObj.Pharmacy.PharmacyAdminUser.FullName,
                    Distributor = orderChargeObj.Distributor.CompanyName,
                    MedicineCount = medicineCount,
                    OrderDate = orderObj.OrderDate.ToShortDateString(),
                    OrderDueDate= orderObj.OrderDate.AddDays(30).ToShortDateString(),
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
                InvoiceDto.MedicineList = new List<MedicineList>();
                if (medicineList.Any())
                {
                    InvoiceDto.MedicineList = medicineList;
                }

                return new ViewAsPdf("DownloadInvoice", InvoiceDto) { FileName = InvoiceDto.UniqueOrder + ".pdf" };

                //return RedirectToAction("DownloadInvoiceAsPDF", InvoiceDto);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get/DownloadInvoice");
                return View();
            }

        }

        #endregion

        #region Common
        public void BindDropDownList()
        {
            ViewBag.OrderStatus = EnumHelpers.EnumToList<GlobalEnums.OrderStatus>().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Value.ToString()
            }).OrderBy(x => x.Text).ToList();
        }

        public async Task SendUpdateOrderEmail(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<string> ToAddressEmailList = new List<string>();
                    var orderCharge = _orderCharge.GetById(id);
                    var Pharmacy = _order.GetById(orderCharge.OrderId).Pharmacy;

                    string physicalUrl = GetPhysicalUrl();
                    string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.UpdateOrder, physicalUrl);
                    emailTemplate = emailTemplate.Replace("{PharmacyNote}", "Your Order Has been " + Enum.GetName(typeof(OrderStatus), orderCharge.OrderStatus));
                    emailTemplate = emailTemplate.Replace("{UserName}", Pharmacy.PharmacyAdminUser.FullName);
                    emailTemplate = emailTemplate.Replace("{Distributor}", orderCharge.Distributor.CompanyName);
                    emailTemplate = emailTemplate.Replace("{orderNumber}", orderCharge.Order.UniqueOrder);
                    emailTemplate = emailTemplate.Replace("{totalCost}", orderCharge.GrandTotal.ToString());

                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                    {
                        Subject = "Order Updates",
                        ToAddress = Pharmacy.PharmacyAdminUser.Email,
                        BodyText = emailTemplate
                    });

                    txscope.Complete();
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/UpdateOrder-Sending Email Exceptions");
                }
            }
        }

        public IActionResult DemoViewAsPDF(InvoiceDto model)
        {
            return new ViewAsPdf("DemoViewAsPDF","OrderPdf.pdf",model);
        }
        public ActionResult DownloadInvoiceAsPDF(InvoiceDto model)
        {
            return new ViewAsPdf(
                           "DownloadInvoice", model){ FileName = model.UniqueOrder+".pdf"};
        }

        public void GanerateOrderStatusList(short currentStatus) {
            var OrderStatusList = EnumHelpers.EnumToList<GlobalEnums.OrderStatus>().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Value.ToString()
            }).OrderBy(x => x.Text).ToList();
            List<OrderStatus> statusList = new List<OrderStatus>();
            switch (currentStatus) 
            {
             case  (short)OrderStatus.Pending:
                                    statusList.Add(OrderStatus.Confirmed);
                                    statusList.Add(OrderStatus.Cancelled);
                            break;
             case (short)OrderStatus.Confirmed:
                                    statusList.Add(OrderStatus.Shipped);
                                    statusList.Add(OrderStatus.Cancelled);
                            break;
             case (short)OrderStatus.Shipped:
                                    statusList.Add(OrderStatus.Delivered);
                                    statusList.Add(OrderStatus.Cancelled);
                            break;
             case (short)OrderStatus.Cancelled:
                                    
                            break;
             case (short)OrderStatus.Delivered:
                                    statusList.Add(OrderStatus.Returned);
                            break;
             }
            ViewBag.OrderStatus = statusList.Select(x => new SelectListItem {
                                   Text = x.ToString(),
                                   Value = EnumHelpers.GetValueFromDescription<OrderStatus>(x.ToString()).ToString()
                                }).OrderBy(x => x.Text).ToList();    
        }
        #endregion
    }
}