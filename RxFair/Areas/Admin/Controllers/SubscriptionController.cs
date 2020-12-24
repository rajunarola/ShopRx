using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        private readonly ISubscriptionTypeHistoryService _subscriptionTypeHistory;
        private readonly IDistributorService _distributor;
        private readonly IDistributorSubscriptionService _distributorSubscription;
        private readonly IDistributorSubscriptionHistoryService _distributorSubscriptionHistory;
        private readonly EmailService _emailService;
        private readonly ISubscriptionTypeService _subscriptionType;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(ISubscriptionTypeHistoryService subscriptionTypeHistory, IDistributorService distributor, IDistributorSubscriptionService distributorSubscription,
            IDistributorSubscriptionHistoryService distributorSubscriptionHistory, IOptions<EmailSettingsGmail> emailSettingsGmail, ISubscriptionTypeService subscriptionType,
            UserManager<ApplicationUser> userManager)
        {
            _subscriptionTypeHistory = subscriptionTypeHistory;
            _distributor = distributor;
            _distributorSubscription = distributorSubscription;
            _distributorSubscriptionHistory = distributorSubscriptionHistory;
            _emailService = new EmailService(emailSettingsGmail);
            _subscriptionType = subscriptionType;
            _userManager = userManager;
        }

        #region Subscription Type
        public IActionResult SubscriptionType()
        {
            return View();
        }

        public async Task<IActionResult> GetSubscriptionTypeList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _subscriptionType.GetSubscriptionTypeListAsync(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetSubscriptionTypeList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult EditSubscriptionType(long id)
        {
            if (id == 0) return View(@"Components/_EditSubscriptionType", new SubscriptionTypeDto { Id = id });
            var result = _subscriptionType.GetSingle(x => x.Id == id);
            return View(@"Components/_EditSubscriptionType", new SubscriptionTypeDto
            {
                Id = result.Id,
                SubscriptionTypeName = result.SubscriptionTypeName,
                ChargedMonthly = result.ChargedMonthly,
                SubscriptionCharge = result.SubscriptionCharge,
                Description = result.Description,
                Brand = result.Brand,
                Generic = result.Generic,
                Otc = result.Otc
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubscriptionType(SubscriptionTypeDto model)
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
                    var result = _subscriptionType.GetSingle(x => x.IsActive && x.Id == model.Id);
                    result.Description = model.Description;
                    if (result.ChargedMonthly != model.ChargedMonthly || result.SubscriptionCharge != model.SubscriptionCharge ||
                        result.Brand != model.Brand || result.Generic != model.Generic || result.Otc != model.Otc)
                    {
                        await _subscriptionTypeHistory.InsertAsync(new SubscriptionTypeHistory
                        {
                            SubscriptionTypeId = result.Id,
                            ChargedMonthly = result.ChargedMonthly,
                            SubscriptionCharge = result.SubscriptionCharge,
                            Brand = result.Brand,
                            Generic = result.Generic,
                            Otc = result.Otc,
                            CreatedDate = DateTime.UtcNow,
                            IsActive = true
                        }, Accessor, User.GetUserId());
                        result.ChargedMonthly = model.ChargedMonthly;
                        result.SubscriptionCharge = model.SubscriptionCharge;
                        result.Brand = model.Brand;
                        result.Generic = model.Generic;
                        result.Otc = model.Otc;
                    }
                    await _subscriptionType.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Subscription Type updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EditSubscriptionType");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult CommissionHistory(long id)
        {
            return View();
        }

        public async Task<IActionResult> GetCommissionHistoryList(long id, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@SubscriptionTypeId", SqlDbType.Int) { Value = id });
                var allList = await _subscriptionTypeHistory.GetSubscriptionTypeHistoryList(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetCommissionHistoryList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }
        #endregion

        #region Distributor Subscription
        public IActionResult DistributorSubscription()
        {
            return View();
        }

        public async Task<IActionResult> GetDistributorSubscriptionList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                var allList = await _distributorSubscription.GetDistributorSubscriptionList(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetDistributorSubscriptionList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditDistributorSubscription(long id)
        {
            var subscriptionType = _subscriptionType.GetAll(x => x.IsActive).Select(x => new SubscriptionTypeDto()
            {
                Id = x.Id,
                SubscriptionTypeName = x.SubscriptionTypeName,
                ChargedMonthly = x.ChargedMonthly,
                SubscriptionCharge = x.SubscriptionCharge,
                Description = x.Description,
                Brand = x.Brand,
                Generic = x.Generic,
                Otc = x.Otc
            }).ToList();
            var model = new DistributorSubscriptionDto
            {
                Id = id,
                SubscriptionTypeId = 0,
                SubscriptionTypeDtos = subscriptionType
            };

            if (id == 0)
            {
                ViewBag.DistributorList = _distributor.GetAll().Where(x => x.IsActive && x.DistributorAdminUser.EmailConfirmed && x.CompanyName != null).Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
                return View(model);
            }

            model = Mapper.Map<DistributorSubscriptionDto>(_distributorSubscription.GetSingle(x => x.Id == id));
            //model.Email = _distributor.GetById(model.DistributorId).DistributorAdminUser.Email;
            model.SubscriptionTypeDtos = subscriptionType;
            model.DateStart = model.StartDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
            var distributor = _distributorSubscription.GetAll(x => x.Id == id).FirstOrDefault();
            ViewBag.DistributorList = _distributor.GetAll(x => x.Id == distributor.DistributorId).Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditDistributorSubscription(DistributorSubscriptionDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var userId = User.GetUserId();
                        if (model.Id == 0)
                        {
                            var activeSubscription = _distributorSubscription.GetSingle(x => x.DistributorId == model.DistributorId && x.IsActive);
                            if (activeSubscription == null)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Distributor Is DeActivated !");
                            }

                            activeSubscription.DistributorId = model.DistributorId;
                            activeSubscription.Notes = model.Notes;
                            if (activeSubscription?.SubscriptionTypeId == model.SubscriptionTypeId)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "This distributor has already subscribed in this subscription, please select another subscription !");
                            }

                            var distributorInfo = _distributor.GetById(model.DistributorId);
                            string userName = distributorInfo.DistributorAdminUser.FullName;
                            var paymentUrl = $@"{GetPhysicalUrl()}/Distributor/MyAccount/UpgradSubscription?id={model.DistributorId}&subscriptionTypeId={model.SubscriptionTypeId}&startDate={model.DateStart}";
                            var emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.SubscriptionActivation, GetPhysicalUrl());
                            emailTemplate = emailTemplate.Replace("{action_url}", paymentUrl);
                            emailTemplate = emailTemplate.Replace("{UserName}", userName);

                            await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                            {
                                ToAddress = distributorInfo.Email,
                                ToDisplayName = userName,
                                Subject = "New Subscription Activation Request",
                                BodyText = emailTemplate
                            });
                            await _distributorSubscription.UpdateAsync(activeSubscription, Accessor, userId);
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, "Distributor subscription link sent to distributor email successfully.");

                        }
                        var result = _distributorSubscription.GetSingle(x => x.Id == model.Id);
                        if (result != null)
                        {
                            if (result.SubscriptionTypeId != model.SubscriptionTypeId || result.SubscriptionTypeId > model.SubscriptionTypeId)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Distributor subsription can't be same or downgraded !");
                            }
                            result.SubscriptionTypeId = model.SubscriptionTypeId;
                            result.Notes = model.Notes;
                        }
                        await _distributorSubscription.UpdateAsync(result, Accessor, userId);
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Distributor subscription updated successfully.");
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditDistributorSubscription");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageDistributorSubscriptionStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _distributorSubscription.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _distributorSubscription.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Distributor subscription {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageDistributorSubscriptionStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndDistributorSubscription(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _distributorSubscription.GetSingle(x => x.Id == id);
                    if (result.IsExpire)
                    {
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, $@"Distributor subscription Is Already Expired.", result.Id);
                    }
                    else
                    {
                        result.IsExpire = true;
                        result.IsActive = false;
                        await _distributorSubscription.UpdateAsync(result, Accessor, User.GetUserId());
                        string physicalUrl = GetPhysicalUrl();
                        string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.EndSubscription, physicalUrl);
                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                        {
                            Subject = "Subscription Termination",
                            ToAddress = result.Distributor.Email,
                            BodyText = emailTemplate
                        });
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, $@"Distributor subscription ended successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EndDistributorSubscription");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult GetDistributorAdminEmail(long id)
        {
            var email = _distributor.GetById(id).DistributorAdminUser.Email;
            return JsonResponse.GenerateJsonResult(1, null, email);
        }

        public IActionResult SubscriptionHistory(long id)
        {
            ViewBag.DistributorName = _distributor.GetById(id)?.CompanyName ?? "";
            return View();
        }

        public async Task<IActionResult> GetDistributorSubscriptionHistoryList(long id, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0));
                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.Int) { Value = id });
                var allList = await _distributorSubscriptionHistory.GetDistributorSubscriptionHistoryList(parameters.Parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetDistributorSubscriptionHistoryList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }
        #endregion
    }
}