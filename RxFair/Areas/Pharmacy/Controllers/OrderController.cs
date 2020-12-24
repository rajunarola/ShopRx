using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using RxFair.Data.DbModel;
using RxFair.Data.Utility;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using static RxFair.Dto.Enum.GlobalEnums;

namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class OrderController : BaseController<OrderController>
    {
        #region Fields

        private readonly IManufacturerService _manufacturer;
        private readonly IDosageFormService _dosageForm;
        private readonly IMedicineMasterService _medicineMaster;
        private readonly IMedicinePriceMasterService _medicinePrice;
        private readonly IDistributorService _distributor;
        private readonly IDistributerOrderSettingService _distributerOrderSetting;
        private readonly ICartService _cart;
        private readonly IPharmacyBillingAddressService _pharmacyBilling;
        private readonly IPharmacyShippingAddressService _pharmacyShipping;
        private readonly IPharmacyService _pharmacy;
        private readonly IPharmacyBillingAddressService _billingAddress;
        private readonly IPharmacyShippingAddressService _shippingAddress;
        private readonly IOrderService _order;
        private readonly IDistributorOrderChargeService _orderCharge;
        private readonly IDistributorOrderService _distributorOrder;
        private readonly ITimeZoneService _timeZone;
        private readonly IUserService _user;
        private readonly EmailService _emailService;
        private readonly ICommissionHistoryService _commissionHistory;

        #endregion

        #region Ctor

        public OrderController(ICommissionHistoryService commissionHistory, IPharmacyBillingAddressService billingAddress, IPharmacyShippingAddressService shippingAddress, IManufacturerService manufacturer, IDosageFormService dosageForm, IUserService user, IOptions<EmailSettingsGmail> emailSettingsGmail, ITimeZoneService timeZone, IPharmacyService pharmacy, IOrderService order, IDistributorOrderChargeService orderCharge, IDistributorOrderService distributorOrder, IMedicineMasterService medicineMaster, IDistributorService distributor, IDistributerOrderSettingService distributerOrder, ICartService cart, IMedicinePriceMasterService medicinePrice)
        {
            _commissionHistory = commissionHistory;
            _billingAddress = billingAddress;
            _shippingAddress = shippingAddress;
            _manufacturer = manufacturer;
            _dosageForm = dosageForm;
            _user = user;
            _emailService = new EmailService(emailSettingsGmail);
            _distributorOrder = distributorOrder;
            _timeZone = timeZone;
            _order = order;
            _orderCharge = orderCharge;
            _pharmacy = pharmacy;
            _pharmacyBilling = billingAddress;
            _pharmacyShipping = shippingAddress;
            _medicineMaster = medicineMaster;
            _medicinePrice = medicinePrice;
            _distributor = distributor;
            _distributerOrderSetting = distributerOrder;
            _cart = cart;
        }
        #endregion

        #region Methods

        [HttpGet]
        public IActionResult SearchMedicine()
        {
            ViewBag.IsFooter = false;
            BindDropdownList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MedicineSearch()
        {
            ViewBag.DistributorList = _distributor.GetDistributorAdminList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).OrderBy(x => x.Text).ToList();
            long PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            int perPage = Convert.ToInt32(GetConfigValue("CommonProperty:MedicinePerPage"));
            var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter("@PharmacyId",SqlDbType.BigInt){Value = PharmacyId},
                new SqlParameter("@DistributorId",SqlDbType.BigInt){Value = 0},
                new SqlParameter("@iDisplayStart",SqlDbType.Int){Value = 0},
                new SqlParameter("@iDisplayLength",SqlDbType.Int){Value =perPage },
                new SqlParameter("@Search",SqlDbType.VarChar){Value =""},
                searchRecords
            };

            var allList = await _medicineMaster.MedicineSearch(parameters.ToArray());
            ViewBag.TotalRecords = allList.FirstOrDefault()?.TotalRecords ?? 0;
            ViewBag.SearchRecords = searchRecords.Value;
            ViewBag.Search = "";
            ViewBag.PerPage = perPage;

            return View(allList);
        }

        [HttpGet]
        public async Task<IActionResult> SearchMedicineList(SearchMedicineDto model, JQueryDataTableParamModel param)
        {
            var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            try
            {
                var customSearch = GetDatatableSearchParam(HttpContext.Request, param.iColumns);
                foreach (var item in customSearch)
                {
                    switch (item.SearchColumnName)
                    {
                        case "medicineName":
                            model.MedicineName = item.SearchValue ?? "";
                            break;
                        case "strength":
                            model.Strength = item.SearchValue ?? "";
                            break;
                        case "packageSize":
                            model.PackageSize = float.Parse(item.SearchValue ?? "0");
                            break;
                        case "dosageForm":
                            model.DosageForm = item.SearchValue ?? "";
                            break;
                        case "ndc":
                            model.NDC = item.SearchValue ?? "";
                            break;
                        case "category":
                            model.CategoryType = item.SearchValue ?? "";
                            break;
                        case "awpPrice":
                            model.AwpPrice = float.Parse(item.SearchValue ?? "0");
                            break;
                        case "wacPrice":
                            model.WacPrice = float.Parse(item.SearchValue ?? "0");
                            break;
                        case "distributorName":
                            model.DistributorName = item.SearchValue ?? "";
                            break;
                        default:
                            break;
                    }
                }
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, param.iSortCol_0.ToString());

                parameters.Parameters.Insert(00, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
                parameters.Parameters.Insert(01, new SqlParameter("@DrugName", SqlDbType.NVarChar) { Value = model.MedicineName ?? "" });
                parameters.Parameters.Insert(02, new SqlParameter("@NDC", SqlDbType.NVarChar) { Value = model.NDC ?? "" });
                parameters.Parameters.Insert(03, new SqlParameter("@Category", SqlDbType.NVarChar) { Value = model.Category });
                parameters.Parameters.Insert(04, new SqlParameter("@Contracted", SqlDbType.Bit) { Value = model.IsContracted });
                parameters.Parameters.Insert(05, new SqlParameter("@ShortDated", SqlDbType.Bit) { Value = model.IsShortDated });
                parameters.Parameters.Insert(06, new SqlParameter("@BestDeal", SqlDbType.Bit) { Value = model.IsBestDeal });
                parameters.Parameters.Insert(07, new SqlParameter("@IsCheap", SqlDbType.Bit) { Value = model.IsCheap });
                parameters.Parameters.Insert(08, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = model.DistributorId });

                parameters.Parameters.Insert(09, new SqlParameter("@Strength", SqlDbType.NVarChar) { Value = model.Strength ?? "" });
                parameters.Parameters.Insert(10, new SqlParameter("@PackageSize", SqlDbType.NVarChar) { Value = model.PackageSize.ToString() });
                parameters.Parameters.Insert(11, new SqlParameter("@DosageForm", SqlDbType.NVarChar) { Value = model.DosageForm ?? "" });
                parameters.Parameters.Insert(12, new SqlParameter("@CategoryType", SqlDbType.NVarChar) { Value = model.CategoryType ?? "" });
                parameters.Parameters.Insert(13, new SqlParameter("@AwpPrice", SqlDbType.NVarChar) { Value = model.AwpPrice.ToString() });
                parameters.Parameters.Insert(14, new SqlParameter("@WacPrice", SqlDbType.NVarChar) { Value = model.WacPrice.ToString() });
                parameters.Parameters.Insert(15, new SqlParameter("@DistributorName", SqlDbType.NVarChar) { Value = model.DistributorName ?? "" });


                var allList = await _medicineMaster.SearchMedicineList(parameters.Parameters.ToArray());
                allList.ForEach(x => { x.MedicineImage = GetS3ServiceUrl(BucketName.MedicineImage, x.MedicineImage); });

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
        public async Task<IActionResult> MedicineSearchList(string search, long distributorId = 0, int startFrom = 0)
        {
            try
            {
                long pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                int perPage = Convert.ToInt32(GetConfigValue("CommonProperty:MedicinePerPage"));
                var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                List<SqlParameter> parameters = new List<SqlParameter>()
                {
                  new SqlParameter("@PharmacyId",SqlDbType.BigInt){Value = pharmacyId},
                  new SqlParameter("@DistributorId",SqlDbType.BigInt){Value = distributorId},
                  new SqlParameter("@iDisplayStart",SqlDbType.Int){Value = startFrom},
                  new SqlParameter("@iDisplayLength",SqlDbType.Int){Value =perPage },
                  new SqlParameter("@Search",SqlDbType.VarChar){Value =search??""},
                  searchRecords
               };

                var allList = await _medicineMaster.MedicineSearch(parameters.ToArray());
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                ViewBag.TotalRecords = allList.FirstOrDefault()?.TotalRecords ?? 0;
                ViewBag.SearchRecords = searchRecords.Value;
                ViewBag.Search = "";
                ViewBag.PerPage = perPage;
                var results = new { data = allList, SearchRecords = searchRecords.Value, TotalRecords = total };

                return JsonResponse.GenerateJsonResult(1, "success", results);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post/MedicineSearchList");
                return JsonResponse.GenerateJsonResult(0, "Something went wrong.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddtoCart(List<CartDto> model, bool isMedicineSearch = false)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cartDate = Convert.ToDateTime(DateTime.Now.Date);
                    var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    long userId = User.GetUserId();
                    foreach (var x in model)
                    {
                        if (x.Quantity > 0)
                        {
                            var medicineId = Convert.ToInt64(x.MedicineId);
                            var oldMedicine = _cart.GetAll(y => y.MedicineId == medicineId && y.PharmacyId == pharmacyId && y.DistributorId == x.DistributorId).FirstOrDefault();
                            if (oldMedicine != null)
                            {
                                // UpdateCart
                                oldMedicine.CartDate = cartDate;
                                oldMedicine.Quantity = isMedicineSearch ? (oldMedicine.Quantity + x.Quantity) : x.Quantity;//(oldMedicine.Quantity + x.Quantity);
                                await _cart.UpdateAsync(oldMedicine, Accessor, userId);
                            }
                            else
                            {
                                // Insert Cart
                                x.PharmacyId = pharmacyId;
                                x.CartDate = cartDate;
                                var cartObj = Mapper.Map<Cart>(x);
                                await _cart.InsertAsync(cartObj, Accessor, userId);
                            }
                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Please enter valid quantity !");
                        }
                    }
                    BindDropdownList();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.InsertCart, ViewBag.CartItem);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddtoCart");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> PlaceOrder()
        {
            try
            {
                var model = new ViewPlaceOrderDto();
                var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                var parameters = new List<SqlParameter> { new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId } };
                var medicineList = await _cart.GetCartList(parameters.ToArray());
                model.CartList = medicineList.GroupBy(x => x.DistributorId).ToList();
                var distributorIds = medicineList.DistinctBy(x => x.DistributorId).Select(x => x.DistributorId).ToList();
                parameters = new List<SqlParameter> { new SqlParameter("@DistributorId", SqlDbType.NVarChar) { Value = string.Join(",", distributorIds) } };
                model.DistributorOrderSettings = await _cart.GetDistributorOrderSetting(parameters.ToArray());
                if (model.DistributorOrderSettings.Count > 0)
                {
                    foreach (var orderSetting in model.DistributorOrderSettings)
                    {
                        if (orderSetting.DistributorId == 0)
                        {
                            orderSetting.CutOffTime = null;
                        }
                        else
                        {
                            orderSetting.CutOffTime = GetCurrentCuttOffTime(orderSetting.Id)?.TimeOfDay;
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GET/placeOrder");
                return RedirectToAction("SearchMedicine");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RemoveCart(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var cartObj = await _cart.GetSingleAsync(x => x.Id == id);
                    _cart.Delete(cartObj);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Item removed from cart.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Get/RemoveCart");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult GetPharmacyAddressList()
        {
            var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            var pharmacy = _pharmacy.GetSingle(x => x.Id == pharmacyId);

            var model = new OrderPharmacyAddressListDto
            {
                PharmacyName = pharmacy.PharmacyName,
                PharmacyBillAddresses = Mapper.Map<List<PharmacyBillOrShipAddressDto>>(pharmacy.BillingAddresses),
                PharmacyShipAddresses = Mapper.Map<List<PharmacyBillOrShipAddressDto>>(pharmacy.ShippingAddresses),
            };
            return JsonResponse.GenerateJsonResult(1, "Address Found", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewOrder(OrderDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    long pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    long userId = User.GetUserId();
                    ICollection<DistributorOrderCharge> distributorOrderCharges = new List<DistributorOrderCharge>();
                    var billaddressobj = _pharmacyBilling.GetSingle(x => x.Id == model.BillingAddressId);
                    var shippaddressobj = _pharmacyShipping.GetSingle(x => x.Id == model.ShippingAddressId);
                    var billingAddress = Mapper.Map<PharmacyBillOrShipAddressDto>(billaddressobj);
                    var shippingAddress = Mapper.Map<PharmacyBillOrShipAddressDto>(shippaddressobj);

                    Order orderObj = new Order
                    {
                        BillingAddress = JsonConvert.SerializeObject(Mapper.Map<PharmacyBillOrShipAddressDto>(billaddressobj)),
                        ShippingAddress = JsonConvert.SerializeObject(Mapper.Map<PharmacyBillOrShipAddressDto>(shippaddressobj))
                    };

                    // Distributor Wise Order Charges 

                    foreach (var item in model.DistributorOrders)
                    {
                        ICollection<DistributorOrder> distributorOrders = new List<DistributorOrder>();
                        var distributor = _distributor.GetSingle(x => x.Id == item.DistributorId);
                        orderObj.OrderShippingTotal += item.GTotal > distributor.DistributorOrderSettings.MinOrderAmount ? distributor.DistributorOrderSettings.ShippingCharge : 0;
                        var distributorOrderCharge = new DistributorOrderCharge()
                        {
                            OrderId = orderObj.Id,
                            IsActive = true,
                            CreatedDate = Convert.ToDateTime(DateTime.Now),
                            OrderStatus = (short)GlobalEnums.OrderStatus.Pending,
                            DistributorId = item.DistributorId,
                        };

                        // Add OverNight Charges if Order placed after Distributor's cutt-Off Time     
                        var isOvernight = DateTime.Now.TimeOfDay > GetCurrentCuttOffTime(item.DistributorId)?.TimeOfDay;
                        if (isOvernight)
                        {
                            orderObj.IsOverNight = true;
                            // orderObj.OrderGrandTotal += orderObj.OrderGrandTotal+ distributor.DistributorOrderSettings.OverNightAmount;
                        }

                        // Distributor Wise Medicine Order
                        foreach (var element in item.MedicineList)
                        {
                            var medicinePrice = distributor.MedicinePriceMasters?.FirstOrDefault(x => x.MedicineId == element.MedicineId && x.DistributorId == item.DistributorId)?.WacpackagePrice ?? 0;
                            var medicinePrieTotal = medicinePrice * element.Quantity;
                            orderObj.OrderSubTotal += Convert.ToDecimal(medicinePrieTotal);
                            distributorOrderCharge.SubTotal += Convert.ToDecimal(medicinePrieTotal);
                            var distributorOrder = new DistributorOrder()
                            {
                                DistributorId = item.DistributorId,
                                OrderChargeId = distributorOrderCharge.Id,
                                MedicineId = element.MedicineId,
                                Quantity = element.Quantity,
                                Price = Convert.ToDecimal(medicinePrice),
                                TotalPrice = Convert.ToDecimal(medicinePrieTotal),
                                IsActive = true,
                                CreatedDate = Convert.ToDateTime(DateTime.Now)
                            };
                            distributorOrders.Add(distributorOrder);
                        }
                        distributorOrderCharge.OverNightCharge = isOvernight ? distributor.DistributorOrderSettings.OverNightAmount : 0;
                        distributorOrderCharge.ShippingTotal = item.GTotal > distributor.DistributorOrderSettings.MinOrderAmount ?
                                distributor.DistributorOrderSettings.ShippingCharge : 0;
                        distributorOrderCharge.GrandTotal = (distributorOrderCharge.SubTotal + distributorOrderCharge.ShippingTotal + (decimal)distributorOrderCharge.OverNightCharge);
                        orderObj.OrderGrandTotal += distributorOrderCharge.GrandTotal;
                        distributorOrderCharge.DistributorOrders = distributorOrders;
                        distributorOrderCharge.CommissionCountDate = Convert.ToDateTime(DateTime.Now);
                        distributorOrderCharges.Add(distributorOrderCharge);
                    }

                    orderObj.OrderCharges = distributorOrderCharges;
                    orderObj.IsActive = true;
                    orderObj.CreatedDate = Convert.ToDateTime(DateTime.Now);
                    orderObj.DeliveryDate = Convert.ToDateTime(DateTime.Now.Date).AddDays(1);
                    orderObj.DeliveryStatus = false;
                    // orderObj.OrderGrandTotal += orderObj.OrderSubTotal + orderObj.OrderShippingTotal ;
                    orderObj.OrderDate = Convert.ToDateTime(DateTime.Now);
                    orderObj.PharmacyId = pharmacyId;
                    orderObj.UniqueOrder = DateTime.Now.Ticks.ToString();

                    var insertResult = await _order.InsertAsync(orderObj, Accessor, userId);

                    #region Remove From Cart
                    foreach (var item in model.DistributorOrders)
                    {
                        var mediList = item.MedicineList.Select(x => x.MedicineId).ToList();
                        var cartList = _cart.GetAll(x => x.PharmacyId == pharmacyId && x.DistributorId == item.DistributorId && mediList.Contains(x.MedicineId)).ToList();
                        _cart.DeleteRange(cartList, Accessor, userId);
                    }
                    #endregion

                    var completedOrder = new CompletedOrderDto { DistributorInfo = new List<CompletedOrDistributorInfo>() };

                    var order = _order.GetSingle(x => x.Id == insertResult.Id);

                    foreach (var item in order.OrderCharges)
                    {
                        completedOrder.DistributorInfo.Add(new CompletedOrDistributorInfo
                        {
                            ComapanyLogo = item.Distributor.CompanyLogo,
                            DistributorCompanyName = item.Distributor.CompanyName,
                            OrderTotal = item.GrandTotal
                        });
                    }

                    completedOrder.BillingAddress = billingAddress;
                    completedOrder.ShippingAddress = shippingAddress;
                    completedOrder.OrderNumber = order.UniqueOrder;

                    txscope.Complete();
                    await SendNewOrderEmail(insertResult, model);

                    //#region commission
                    //using (var Innerscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    //{
                    //    try {
                    //        //Insertion in CommissionHistoryTable
                    //        foreach (var item in insertResult.OrderCharges)
                    //            {
                    //            CommissionHistory commissionHistory = new CommissionHistory();
                    //            commissionHistory.OrderChargeId = item.Id;
                    //            var commissionResult=await _commissionHistory.InsertAsync(commissionHistory, Accessor, User.GetUserId());

                    //            var parameters = new List<SqlParameter>
                    //                       {
                    //                         new SqlParameter("@OrderChargeId",SqlDbType.BigInt){Value = item.Id},
                    //                         new SqlParameter("@DistributorId",SqlDbType.BigInt){Value = item.DistributorId},
                    //                         new SqlParameter("@CreatedDate",SqlDbType.VarChar){Value = item.CreatedDate.ToShortDateString()}
                    //                        };
                    //            var CalculatedResult = await _commissionHistory.GetCommission(parameters.ToArray());
                    //            commissionResult.CommissionAmount = CalculatedResult.OrderCommission;
                    //            await _commissionHistory.UpdateAsync(commissionResult, Accessor, User.GetUserId());
                    //            Innerscope.Complete();
                    //        }

                    //        } catch (Exception ex) {
                    //        Innerscope.Dispose();
                    //        ErrorLog.AddErrorLog(ex, "Post/AddNewOrder/Commission_Region");
                    //        return JsonResponse.GenerateJsonResult(0, GlobalConstant.CommissionErrorMessage);
                    //    }
                    //}
                    //    #endregion

                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.OrderPlaced, completedOrder);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddNewOrder");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult ViewAllOrders()
        {
            BindDistributorList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders(JQueryDataTableParamModel param, long DistributorId, string StartDate, string EndDate)
        {
            var PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            try
            {
                if (!string.IsNullOrEmpty(StartDate))
                    StartDate = Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (!string.IsNullOrEmpty(EndDate))
                    EndDate = Convert.ToDateTime(EndDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = PharmacyId });
                parameters.Parameters.Insert(1, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = DistributorId });
                parameters.Parameters.Insert(2, new SqlParameter("@StartDate", SqlDbType.VarChar) { Value = StartDate ?? "" });
                parameters.Parameters.Insert(3, new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = EndDate ?? "" });
                var allList = await _pharmacy.GetPharmacyOrderList(parameters.Parameters.ToArray());
                //var total = allList.Count;
                //allList.ForEach(x =>
                //{
                //    // x.InvoiceFile = $@"{FilePathList.Invoice}\{x.InvoiceFile}";
                //});
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
        public IActionResult ViewOrderDetails(long id)
        {
            var result = _orderCharge.GetSingle(x => x.Id == id);

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
        public IActionResult ViewPurchaseHistory(long id)
        {
            var Medicine = _medicineMaster.GetById(id);
            var dosageForm = _dosageForm.GetSingle(x => x.Id == Medicine.DosageFormId).DosageForm ?? "";
            var manufacturer = _manufacturer.GetSingle(x => x.Id == Medicine.ManufacturerId).ManufacturerName ?? "";

            MedicinePurchaseHistoryViewDto model = new MedicinePurchaseHistoryViewDto();
            model.Id = Medicine.Id;
            model.MedicineName = Medicine.DrugName;
            model.Strength = Medicine.Strength;
            model.DosageForm = dosageForm ?? "";
            model.Category = Medicine.CategoryMaster?.MedicineCategory ?? "";
            model.Manufacturer = manufacturer;
            model.Flavour = Medicine.Flavour ?? "";

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicinePurchaseHistory(JQueryDataTableParamModel param, long id)
        {
            var PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = PharmacyId });
                parameters.Parameters.Insert(1, new SqlParameter("@MedicineId", SqlDbType.BigInt) { Value = id });
                var allList = await _orderCharge.GetMedicinePurchaseHistory(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetMedicinePurchaseHistory");
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
        [Route("/Pharmacy/Order/DownloadInvoice/{orderChargeId}/{uniqueId}")]
        public IActionResult DownloadInvoice(long orderChargeId, string uniqueId)
        {
            try
            {
                var PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));

                var orderObj = _order.GetSingle(x => x.UniqueOrder == uniqueId);
                var orderChargeObj = _orderCharge.GetSingle(x => x.Id == orderChargeId);
                var distributor = orderChargeObj.Distributor.CompanyName;
                var pharmacy = _pharmacy.GetById(PharmacyId).PharmacyAdminUser.FullName;
                var medicineCount = orderChargeObj.DistributorOrders.Select(x => x.MedicineId).Count();
                InvoiceDto invoiceDto = new InvoiceDto
                {
                    UniqueOrder = orderChargeObj.Order.UniqueOrder,
                    Pharmacy = orderChargeObj.Order.Pharmacy.PharmacyName,
                    Distributor = orderChargeObj.Distributor.CompanyName,
                    OrderDate = orderChargeObj.Order.OrderDate.ToDefaultDateTime(GlobalFormates.DefaultDate),
                    OrderDueDate = orderChargeObj.Order.OrderDate.AddDays(30).ToDefaultDateTime(GlobalFormates.DefaultDate),
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

                return new ViewAsPdf("DownloadInvoice", invoiceDto) { FileName = invoiceDto.UniqueOrder + ".pdf" };
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get/DownloadInvoice");
                return View("Error", "Home");
            }
        }

        #endregion

        #region Common
        public async Task<JsonResult> GetMedicineList(string Prefix, long distributorId, string search)
        {
            try
            {
                var parameters = new List<SqlParameter>();
                parameters.Insert(0, new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = search == null ? Prefix : search });
                parameters.Insert(1, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });

                var medicineList = await _medicineMaster.GetMedicineNameList(parameters.ToArray());
                return Json(medicineList);
            }
            catch (Exception e)
            {
                ErrorLog.AddErrorLog(e, "GetMedicineList");
                return Json("Exeption ");
            }
        }

        private void BindDropdownList()
        {
            ViewBag.DistributorList = _distributor.GetDistributorAdminList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).OrderBy(x => x.Text).ToList();
            ViewBag.CartItem = _cart.GetAll(x => x.PharmacyId == Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId))).Count();
        }

        public void BindDistributorList()
        {
            ViewBag.OrderDistributorList = _pharmacy.GetOrderDistributorList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).OrderBy(x => x.Text).ToList();
        }

        public DateTime? GetCurrentCuttOffTime(long DistributorId)
        {
            var result = new DateTime?();
            var distributorObj = _distributor.GetSingle(x => x.Id == DistributorId).DistributorOrderSettings;
            // TimeZoneInfo.ConvertTime(DateTime., TimeZoneInfo.FindSystemTimeZoneById(distributorObj.TimeZoneMaster.StandardName)))
            string timeZoneId = distributorObj?.TimeZoneMaster?.TimeZoneId ?? "";
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    result = (distributorObj.ServiceDayMonday ? distributorObj?.MondayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Tuesday:
                    result = (distributorObj.ServiceDayTuesday ? distributorObj?.TuesdayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Wednesday:
                    result = (distributorObj.ServiceDayWednesday ? distributorObj?.WednesdayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Thursday:
                    result = (distributorObj.ServiceDayThursday ? distributorObj?.ThursdayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Friday:
                    result = (distributorObj.ServiceDayFriday ? distributorObj?.FridayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Saturday:
                    result = (distributorObj.ServiceDaySaturday ? distributorObj?.SaturdayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
                case DayOfWeek.Sunday:
                    result = (distributorObj.ServiceDaySunday ? distributorObj?.SundayCutOffTime.DistributorToPharmacyUserTime(timeZoneId) : (DateTime?)null);
                    break;
            }
            return result;
        }

        public async Task SendNewOrderEmail(Order insertResult, OrderDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    List<string> ToAddressEmailList = new List<string>();
                    var pharmacy = _pharmacy.GetById(pharmacyId).PharmacyAdminUser;

                    string physicalUrl = GetPhysicalUrl();
                    string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.CreateNewOrder, physicalUrl);
                    emailTemplate = emailTemplate.Replace("{Pharmacy Name}", pharmacy.FullName);
                    emailTemplate = emailTemplate.Replace("{Invoice Date}", insertResult.OrderDate.ToShortDateString());
                    emailTemplate = emailTemplate.Replace("{Invoice Due Date}", insertResult.OrderDate.AddDays(30).ToShortDateString());
                    emailTemplate = emailTemplate.Replace("{OrderNumber}", insertResult.UniqueOrder);
                    string pharmacyOrder = string.Empty;
                    bool orderChargeFlag = false;
                    bool isChargeNoteRemoved = false;
                    int count = 1;
                    foreach (var list in model.DistributorOrders)
                    {
                        var distributorOrderSettingObj = _distributerOrderSetting.GetSingle(x => x.DistributorId == list.DistributorId);
                        string orderTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.InnerNewOrder, physicalUrl);
                        string MedicineDetails = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.MedicineDetails, physicalUrl);

                        var orderTotal = insertResult.OrderCharges.Where(x => x.DistributorId == list.DistributorId).Select(x => x.GrandTotal).FirstOrDefault();
                        var distributorOverNightCharge = insertResult.OrderCharges.FirstOrDefault(x => x.DistributorId == list.DistributorId);

                        string medinfoTamplate = "";
                        string tempMedicineTemplate = "";
                        foreach (var medInfo in list.MedicineList)
                        {
                            tempMedicineTemplate = MedicineDetails;
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{MedicineName}", medInfo.MedicineName);
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{Quantity}", medInfo.Quantity.ToString());
                            tempMedicineTemplate = tempMedicineTemplate.Replace("{Price}", medInfo.MedicinePrice.ToString("#.00"));
                            medinfoTamplate = medinfoTamplate + tempMedicineTemplate;
                        }


                        orderTemplate = orderTemplate.Replace("{MedicineDetails}", medinfoTamplate);
                        orderTemplate = orderTemplate.Replace("{CompanyLogo}", list.CompanyLogo);
                        orderTemplate = orderTemplate.Replace("{Count}", count.ToString());
                        orderTemplate = orderTemplate.Replace("{DistributorName}", list.DistributorName);

                        orderTemplate = orderTemplate.Replace("{OrderTotal}", orderTotal.ToString("#.00"));
                        orderTemplate = orderTemplate.Replace("{MedicineCount}", list.MedicineList.Count.ToString());
                        orderTemplate = orderTemplate.Replace("{ShippingCharges}", list.GTotal > distributorOrderSettingObj.MinOrderAmount ? distributorOrderSettingObj.ShippingCharge.ToString("#.00") : "0");
                        if (distributorOverNightCharge?.OverNightCharge > 0)
                        {
                            orderChargeFlag = true;
                            orderTemplate = orderTemplate.Replace("{overNightChargeLable}", EmailTagMessageList.OverNightChargeLabel);
                            orderTemplate = orderTemplate.Replace("{OverNightCharge}", "$ " + list.OverNightCharge.ToString("#.00"));
                            if (isChargeNoteRemoved)
                                emailTemplate = emailTemplate.Replace("<h6></h6>", EmailTagMessageList.OverNightChargeNote);
                            else
                                emailTemplate = emailTemplate.Replace("{OverNight_Note}", EmailTagMessageList.OverNightChargeNote);
                        }
                        else
                        {
                            isChargeNoteRemoved = true;
                            orderTemplate = orderTemplate.Replace("{overNightChargeLable}", "");
                            orderTemplate = orderTemplate.Replace("{OverNightCharge}", "");
                            emailTemplate = emailTemplate.Replace("{OverNight_Note}", "<h6></h6>");
                        }

                        string distributorOrder = emailTemplate.Replace("{InnerNewOrder}", orderTemplate);
                        var distributor = _distributor.GetById(list.DistributorId).DistributorAdminUser;
                        distributorOrder = distributorOrder.Replace("{UserName}", distributor.FullName);
                        // distributorOrder = distributorOrder.Replace("{MedicineCount}", list.MedicineList.Count.ToString());
                        distributorOrder = distributorOrder.Replace("{PharmacyNote}", "Order has been Successfully Created by " + pharmacy.FullName + " Pharmacy");
                        distributorOrder = distributorOrder.Replace("{Grand Total}", orderTotal.ToString("#.00"));

                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                        {
                            Subject = "New order created !",
                            ToAddress = distributor.Email,
                            BodyText = distributorOrder
                        });
                        orderTemplate = orderTemplate.Replace("{PharmacyNote}", "");
                        pharmacyOrder = pharmacyOrder + orderTemplate;
                        count++;
                    }
                    if (orderChargeFlag)
                    {
                        emailTemplate = emailTemplate.Replace("<h6></h6>", " <h6>" + EmailTagMessageList.OverNightChargeNote + "</h6>");
                    }
                    emailTemplate = emailTemplate.Replace("{InnerNewOrder}", pharmacyOrder);
                    emailTemplate = emailTemplate.Replace("{PharmacyNote}", "");
                    emailTemplate = emailTemplate.Replace("{UserName}", pharmacy.FullName);
                    emailTemplate = emailTemplate.Replace("{Grand Total}", insertResult.OrderGrandTotal.ToString());

                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                    {
                        Subject = "New order created !",
                        ToAddress = pharmacy.Email,
                        BodyText = emailTemplate
                    });
                    txscope.Complete();
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddNewOrder-Sending Email Exceptions");
                }
            }
        }

        public ActionResult DownloadInvoiceAsPDF(InvoiceDto invoiceDto)
        {
            return View("DownloadInvoice", invoiceDto);
        }

        public static List<CustomSearch> GetDatatableSearchParam(HttpRequest request, int columns)
        {
            var customSearchParam = new List<CustomSearch>();
            try
            {
                if (columns < 0) return customSearchParam;
                for (var i = 0; i < columns; i++)
                {
                    var searchColumnName = request.Query["mDataProp_" + i];
                    var searchValue = request.Query["sSearch_" + i];
                    if (string.IsNullOrEmpty(searchValue)) continue;
                    var search = new CustomSearch()
                    {
                        SearchValue = System.Uri.UnescapeDataString(searchValue),
                        SearchColumnName = searchColumnName
                    };
                    customSearchParam.Add(search);
                }

                return customSearchParam;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion
    }
}