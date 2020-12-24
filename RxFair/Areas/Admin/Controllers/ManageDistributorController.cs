using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Dtos;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Data.DbModel;
using RxFair.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RxFair.Dto.Enum;
using RxFair.Service.Utility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class ManageDistributorController : BaseController<ManageDistributorController>
    {
        #region Fields
        private readonly IDistributorService _distributer;
        private readonly IDistributerOrderSettingService _distributerOrder;
        private readonly IStateService _state;
        private readonly IDistributorSubscriptionService _distributorSubscription;
        private readonly IDistributorSubscriptionHistoryService _distributorSubscriptionHistory;
        private readonly ISubscriptionTypeService _subscriptionType;
        private readonly ITimeZoneService _timeZone;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailService _emailService;
        #endregion

        #region Ctor
        public ManageDistributorController(IDistributorService distributer, IDistributerOrderSettingService distributerOrderSetting, IStateService state, ISubscriptionTypeService subscriptionType, ITimeZoneService timeZone,
            IDistributorSubscriptionService distributorSubscription, IDistributorSubscriptionHistoryService distributorSubscriptionHistory, UserManager<ApplicationUser> userManager, IOptions<EmailSettingsGmail> emailSettingsGmail)
        {
            _distributer = distributer;
            _distributerOrder = distributerOrderSetting;
            _state = state;
            _emailService = new EmailService(emailSettingsGmail);
            _subscriptionType = subscriptionType;
            _timeZone = timeZone;
            _distributorSubscription = distributorSubscription;
            _distributorSubscriptionHistory = distributorSubscriptionHistory;
            _userManager = userManager;
        }
        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributerList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allslotList = await _distributer.GetDistributorList(parameters.ToArray());
                var total = allslotList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allslotList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get-GetDistributerList");
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
        public async Task<IActionResult> ManageIsActive(long id)
        {
            try
            {
                var distributor = _distributer.GetSingle(x => x.Id == id);
                distributor.IsActive = !distributor.IsActive;
                distributor.DistributorAdminUser.IsActive = distributor.IsActive;
                await _distributer.UpdateAsync(distributor, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Distributor {(distributor.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-ManageIsActive");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpGet]
        [Route("Admin/ManageDistributor/AddEditDistributor/{id?}")]
        [Route("Admin/ManageDistributor/ViewDistributer/{id}")]
        public async Task<IActionResult> AddEditDistributor(long id = 0)
        {
            string requestedUrl = HttpContext.Request.Path.Value;
            ViewBag.Action = requestedUrl.Contains("ViewDistributer");
            try
            {
                var subscriptionType = _subscriptionType.GetAll(x => x.IsActive).Select(x => new SubscriptionTypeDto()
                {
                    Id = x.Id,
                    SubscriptionTypeName = x.SubscriptionTypeName,
                    ChargedMonthly = x.ChargedMonthly,
                    Description = x.Description,
                    Brand = x.Brand,
                    Generic = x.Generic,
                    Otc = x.Otc
                }).ToList();

                BindDropdownList();
                var model = new DistributorDto
                {
                    Id = id,
                    SubscriptionDto = new DistributorSubscriptionDto { SubscriptionTypeDtos = subscriptionType }
                };
                if (id == 0) return View(model);
                var dit = await _distributer.GetSingleAsync(x => x.Id == id);
                if (dit == null) return RedirectToAction("Index", "ManageDistributor");
                model = Mapper.Map<DistributorDto>(dit);
                model.FaxNumber = dit.DistributorAdminUser?.FaxNumber ?? "";
                model.FirstName = dit.DistributorAdminUser?.FirstName ?? "";
                model.LastName = dit.DistributorAdminUser?.LastName ?? "";
                model.UserEmail = dit.DistributorAdminUser?.Email ?? "";
                model.Email = model.UserEmail;
                model.SubscriptionDto = dit.DistributorSubscriptions.Count != 0
                    ? Mapper.Map<DistributorSubscriptionDto>(dit.DistributorSubscriptions?.FirstOrDefault(x => x.DistributorId == dit.Id))
                    : new DistributorSubscriptionDto() { DistributorId = dit.Id };
                model.SubscriptionDto.SubscriptionTypeDtos = subscriptionType;
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get-AddEditDistributor");
                return RedirectToAction("Index", "ManageDistributor");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditDistributor(DistributorDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }
                    var subscriptionType = _subscriptionType.GetById(model.SubscriptionDto.SubscriptionTypeId);
                    if (model.Id == 0)
                    {
                        var newDistributer = new ApplicationUser
                        {
                            Email = model.Email,
                            UserName = model.UserEmail,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            PhoneNumber = model.Phone,
                            Mobile = model.Mobile,
                            FaxNumber = model.FaxNumber,
                            IsActive = true
                        };
                        string password = CommonMethod.CreateRandomPassword(8);
                        var result = await _userManager.CreateAsync(newDistributer, password);
                        if (!result.Succeeded)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, GlobalConstant.NotRegistered);
                        }
                        await _userManager.AddToRoleAsync(newDistributer, UserRoles.DistributorPrimaryAdmin);

                        var distributer = new Data.DbModel.Distributor
                        {
                            CompanyName = model.CompanyName,
                            Mobile = model.Mobile,
                            Phone = model.Phone,
                            ZipCode = model.ZipCode,
                            Address = model.Address,
                            StateId = model.StateId,
                            City = model.City,
                            ContactName = model.ContactName ?? "",
                            ContactEmail = model.ContactEmail,
                            ContactMobile = model.ContactMobile ?? "",
                            ContactAddress = model.ContactAddress ?? "",
                            ContactCity = model.ContactCity,
                            ContactStateId = model.ContactStateId,
                            ContactZipCode = model.ContactZipCode,
                            Email = model.Email,
                            UserId = newDistributer.Id,
                            IsActive = true
                        };
                        await _distributer.InsertAsync(distributer, Accessor, User.GetUserId());
                        newDistributer.DistributorId = distributer.Id;
                        await _userManager.UpdateAsync(newDistributer);

                        var distributerSubscription = new DistributorSubscription
                        {
                            DistributorId = distributer.Id,
                            Notes = model.SubscriptionDto.Notes,
                            StartDate = model.SubscriptionDto.StartDate,
                            SubscriptionTypeId = model.SubscriptionDto.SubscriptionTypeId,
                            ChargedMonthly = subscriptionType.ChargedMonthly,
                            SubscriptionCharge = subscriptionType.SubscriptionCharge,
                            Brand = subscriptionType.Brand,
                            Generic = subscriptionType.Generic,
                            Otc = subscriptionType.Otc,
                            IsActive = true,
                            IsPayment = false
                        };
                        await _distributorSubscription.InsertAsync(distributerSubscription, Accessor, User.GetUserId());

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(newDistributer);
                        var callbackUrl = Url.EmailConfirmationLink(newDistributer.Id, code, Request.Scheme);

                        var emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.CreateDistributor, GetPhysicalUrl());
                        emailTemplate = emailTemplate.Replace("{UserName}", newDistributer.FullName);
                        emailTemplate = emailTemplate.Replace("{Password}", password);
                        emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);
                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                        {
                            ToAddress = newDistributer.Email,
                            Subject = "Distributor created successfully",
                            BodyText = emailTemplate
                        });

                        //#region Congratulation email
                        //var congratulationTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.Congratulation, GetPhysicalUrl());
                        //congratulationTemplate = congratulationTemplate.Replace("{acounttype}", "Distributer");
                        //congratulationTemplate = congratulationTemplate.Replace("{email}", newDistributer.Email);
                        //await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                        //{
                        //    ToAddress = newDistributer.Email,
                        //    Subject = "Congratulation",
                        //    BodyText = congratulationTemplate
                        //});
                        //#endregion
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Distributor inserted successfully.", User.GetUserId());
                    }

                    var editDistributer = _distributer.GetSingle(x => x.Id == model.Id);
                    editDistributer.CompanyName = model.CompanyName;
                    editDistributer.ContactAddress = model.ContactAddress;
                    editDistributer.ContactCity = model.ContactCity;
                    editDistributer.ContactStateId = model.ContactStateId;
                    editDistributer.ContactZipCode = model.ContactZipCode;
                    editDistributer.ContactEmail = model.ContactEmail;
                    editDistributer.Address = model.Address;
                    editDistributer.DistributorAdminUser.FaxNumber = model.FaxNumber;
                    editDistributer.ContactMobile = model.ContactMobile;
                    editDistributer.ContactName = model.ContactName;
                    editDistributer.DistributorAdminUser.Email = model.Email;
                    editDistributer.Email = model.Email;
                    editDistributer.Mobile = model.Mobile;
                    editDistributer.DistributorAdminUser.Mobile = model.Mobile;
                    editDistributer.Phone = model.Phone;
                    editDistributer.DistributorAdminUser.PhoneNumber = model.Phone;
                    editDistributer.City = model.City;
                    editDistributer.StateId = model.StateId;
                    editDistributer.ZipCode = model.ZipCode;
                    //editDistributer.DistributorAdminUser.FirstName = model.FirstName;
                    //editDistributer.DistributorAdminUser.LastName = model.LastName;
                    //editDistributer.DistributorAdminUser.UserName = model.UserEmail;

                    if (model.SubscriptionDto.Id == 0)
                    {
                        var disSubscription = new DistributorSubscription
                        {
                            DistributorId = model.Id,
                            SubscriptionTypeId = model.SubscriptionDto.SubscriptionTypeId,
                            Notes = model.SubscriptionDto.Notes,
                            ChargedMonthly = subscriptionType.ChargedMonthly,
                            SubscriptionCharge = subscriptionType.SubscriptionCharge,
                            Brand = subscriptionType.Brand,
                            Generic = subscriptionType.Generic,
                            Otc = subscriptionType.Otc,
                        };
                        await _distributorSubscription.InsertAsync(disSubscription, Accessor, User.GetUserId());
                    }
                    else
                    {
                        var editSubscriptions = editDistributer.DistributorSubscriptions.FirstOrDefault(x => x.Id == model.SubscriptionDto.Id);

                        if (editSubscriptions != null)
                        {
                            if (editSubscriptions.SubscriptionTypeId != model.SubscriptionDto.SubscriptionTypeId && model.SubscriptionDto.SubscriptionTypeId > editSubscriptions.SubscriptionTypeId)
                            {
                                // User Subsription Cannot be Same or Downgraded !
                                var history = new DistributorSubscriptionHistory
                                {
                                    DistributorId = editSubscriptions.DistributorId,
                                    Notes = editSubscriptions.Notes,
                                    SubscriptionTypeId = editSubscriptions.SubscriptionTypeId,
                                    StartDate = editSubscriptions.StartDate,
                                    EndDate = editSubscriptions.EndDate,
                                    ChargedMonthly = editSubscriptions.ChargedMonthly,
                                    SubscriptionCharge = editSubscriptions.SubscriptionCharge,
                                    Brand = editSubscriptions.Brand,
                                    Generic = editSubscriptions.Generic,
                                    Otc = editSubscriptions.Otc,
                                    IsPayment = editSubscriptions.IsPayment,
                                    PaymentDate = editSubscriptions.PaymentDate,
                                    PayPalTransactionId = editSubscriptions.PayPalTransactionId
                                };
                                await _distributorSubscriptionHistory.InsertAsync(history, Accessor, User.GetUserId());

                                editSubscriptions.SubscriptionTypeId = model.SubscriptionDto.SubscriptionTypeId;
                                editSubscriptions.DistributorId = editDistributer.Id;
                                editSubscriptions.Notes = model.SubscriptionDto.Notes;
                                editSubscriptions.ChargedMonthly = subscriptionType.ChargedMonthly;
                                editSubscriptions.SubscriptionCharge = subscriptionType.SubscriptionCharge;
                                editSubscriptions.Brand = subscriptionType.Brand;
                                editSubscriptions.Generic = subscriptionType.Generic;
                                editSubscriptions.Otc = subscriptionType.Otc;

                                await _distributorSubscription.UpdateAsync(editSubscriptions, Accessor, User.GetUserId());
                            }
                            else
                            {
                                if (editSubscriptions.SubscriptionTypeId == model.SubscriptionDto.SubscriptionTypeId)
                                    return JsonResponse.GenerateJsonResult(1, "Distributor updated successfully.");
                                //return JsonResponse.GenerateJsonResult(0, "Distributor subsription cannot be downgraded !");
                            }

                        }
                    }

                    await _distributer.UpdateAsync(editDistributer, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Distributor updated successfully.", editDistributer.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditDistributor");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult RemoveDistributer(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //    var distributer = _distributer.GetSingle(x => x.Id == id);
                    //    var user = await _userManager.FindByIdAsync(id.ToString());

                    //    var distributerSubscription = await _distributorSubscription.GetSingleAsync(x=>x.DistributorId== distributer.Id);
                    //    _distributorSubscription.Delete(distributerSubscription);
                    //    var removeRole = await _userManager.RemoveFromRoleAsync(user, UserRoles.DistributorPrimaryAdmin);
                    //    await _userManager.DeleteAsync(user);
                    //    _distributer.Delete(distributer);

                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Distributor deleted successfully.", id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Get-RemoveDistributer");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult EditOrderSetting(long id)
        {
            BindDropdownList(true);
            var orderSetting = _distributerOrder.GetSingle(x => x.DistributorId == id);
            if (orderSetting == null) return View(@"Components/_EditOrderSetting", new DistributerOrderSettingDto { Id = 0, DistributorId = id });

            var model = Mapper.Map<DistributerOrderSettingDto>(orderSetting);
            return View(@"Components/_EditOrderSetting", model);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderSetting(DistributerOrderSettingDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }
                    var editOrder = _distributerOrder.GetSingle(x => x.DistributorId == model.DistributorId);
                    if (editOrder == null)
                    {
                        var orderSetting = new DistributorOrderSetting
                        {
                            DistributorId = model.DistributorId,
                            TimeZoneId = model.TimeZoneId,
                            ServiceDayMonday = model.ServiceDayMonday,
                            ServiceDayTuesday = model.ServiceDayTuesday,
                            ServiceDayWednesday = model.ServiceDayWednesday,
                            ServiceDayThursday = model.ServiceDayThursday,
                            ServiceDayFriday = model.ServiceDayFriday,
                            ServiceDaySaturday = model.ServiceDaySaturday,
                            ServiceDaySunday = model.ServiceDaySunday,
                            MinOrderAmount = model.MinOrderAmount,
                            OverNightAmount = model.OverNightAmount,
                            ShippingCharge = model.ShippingCharge,
                            IsActive = true,
                            MondayCutOffTime =
                            model.ServiceDayMonday
                                ? Convert.ToDateTime(model.MondayCutOffTime).TimeOfDay
                                : (TimeSpan?)null,
                            TuesdayCutOffTime = model.ServiceDayTuesday
                                ? Convert.ToDateTime(model.TuesdayCutOffTime).TimeOfDay
                                : (TimeSpan?)null,
                            WednesdayCutOffTime = model.ServiceDayWednesday
                                ? Convert.ToDateTime(model.WednesdayCutOffTime).TimeOfDay
                                : (TimeSpan?)null,
                            ThursdayCutOffTime = model.ServiceDayThursday
                                ? Convert.ToDateTime(model.ThursdayCutOffTime).TimeOfDay
                                : (TimeSpan?)null,
                            FridayCutOffTime =
                                model.ServiceDayFriday
                                    ? Convert.ToDateTime(model.FridayCutOffTime).TimeOfDay
                                    : (TimeSpan?)null,
                            SaturdayCutOffTime = model.ServiceDaySaturday
                                ? Convert.ToDateTime(model.SaturdayCutOffTime).TimeOfDay
                                : (TimeSpan?)null,
                            SundayCutOffTime =
                                model.ServiceDaySunday
                                    ? Convert.ToDateTime(model.SundayCutOffTime).TimeOfDay
                                    : (TimeSpan?)null
                        };


                        await _distributerOrder.InsertAsync(orderSetting, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Distributor Order Settings created successfully.");
                    }

                    editOrder.DistributorId = model.DistributorId;
                    editOrder.TimeZoneId = model.TimeZoneId;

                    editOrder.ShippingCharge = model.ShippingCharge;
                    editOrder.OverNightAmount = model.OverNightAmount;

                    editOrder.ServiceDayMonday = model.ServiceDayMonday;
                    editOrder.ServiceDayTuesday = model.ServiceDayTuesday;
                    editOrder.ServiceDayWednesday = model.ServiceDayWednesday;
                    editOrder.ServiceDayThursday = model.ServiceDayThursday;
                    editOrder.ServiceDayFriday = model.ServiceDayFriday;
                    editOrder.ServiceDaySaturday = model.ServiceDaySaturday;
                    editOrder.ServiceDaySunday = model.ServiceDaySunday;

                    editOrder.MondayCutOffTime = model.ServiceDayMonday ? Convert.ToDateTime(model.MondayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.TuesdayCutOffTime = model.ServiceDayTuesday ? Convert.ToDateTime(model.TuesdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.WednesdayCutOffTime = model.ServiceDayWednesday ? Convert.ToDateTime(model.WednesdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.ThursdayCutOffTime = model.ServiceDayThursday ? Convert.ToDateTime(model.ThursdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.FridayCutOffTime = model.ServiceDayFriday ? Convert.ToDateTime(model.FridayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.SaturdayCutOffTime = model.ServiceDaySaturday ? Convert.ToDateTime(model.SaturdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.SundayCutOffTime = model.ServiceDaySunday ? Convert.ToDateTime(model.SundayCutOffTime).TimeOfDay : (TimeSpan?)null;

                    await _distributerOrder.UpdateAsync(editOrder, Accessor, User.GetUserId());
                    txscope.Complete();

                    return JsonResponse.GenerateJsonResult(1, "Distributor Order Settings updated successfully.", editOrder.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EditOrderSetting");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CheckUserIsExist(string email, string contactEmail, string userEmail)
        {
            string reuslt = string.Empty;
            if (!string.IsNullOrEmpty(email))
                reuslt = email;
            if (!string.IsNullOrEmpty(contactEmail))
                reuslt = contactEmail;
            if (!string.IsNullOrEmpty(userEmail))
                reuslt = userEmail;

            var isExist = _distributer.GetSingle(x => x.Email.Equals(reuslt));
            return JsonResponse.GenerateJsonResult(isExist != null ? 1 : 0, isExist != null ? GlobalConstant.AlreadyRegisterd : "");
        }

        #endregion

        #region Controller Common
        private void BindDropdownList(bool isSetting = false)
        {
            if (isSetting)
                ViewBag.TimeZoneList = _timeZone.GetAll().Select(x => new SelectListItem { Text = x.StandardName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            else
                ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion

    }
}