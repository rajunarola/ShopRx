using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class RewardController : BaseController<RewardController>
    {
        #region Fields
        private readonly IUserService _user;
        private readonly IRedeemRequestService _redeemRequest;
        private readonly IRewardMonthDaysService _rewardMonthDays;
        private readonly IRewardEarnService _rewardEarn;
        private readonly IRewardTypeMasterService _rewardTypeMaster;
        private readonly IRewardProductService _rewardProduct;
        private readonly IRewardMoneyMasterService _rewardMoney;
        private readonly EmailService _emailService;
        #endregion

        #region Ctor

        public RewardController(IOptions<EmailSettingsGmail> emailSettingsGmail, IUserService user, IRedeemRequestService redeemRequest, IRewardMonthDaysService rewardMonthDays, IRewardEarnService rewardEarn, IRewardTypeMasterService rewardTypeMaster, IRewardProductService rewardProduct, IRewardMoneyMasterService rewardMoney)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _user = user;
            _redeemRequest = redeemRequest;
            _rewardMonthDays = rewardMonthDays;
            _rewardEarn = rewardEarn;
            _rewardTypeMaster = rewardTypeMaster;
            _rewardProduct = rewardProduct;
            _rewardMoney = rewardMoney;
        }
        #endregion

        #region Methods

        #region Reward Point
        public IActionResult RewardMoney()
        {
            var model = _rewardMoney.GetRewardMoneyList();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RewardMoney(List<RewardMoneyDto> model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var isChanges = false;
                try
                {
                    var allIds = model.Select(x => x.Id).ToList();
                    var all = _rewardMoney.GetAll(x => allIds.Contains(x.Id) && x.IsActive).ToList();
                    foreach (var item in model)
                    {
                        var getSingle = all.FirstOrDefault(x => x.Id == item.Id && x.RewardTypeId == item.RewardTypeId);
                        if (getSingle == null) continue;
                        getSingle.Referral = item.Referral;
                        await _rewardMoney.UpdateAsync(getSingle, Accessor, User.GetUserId());
                        isChanges = true;
                    }

                    if (isChanges)
                        txscope.Complete();
                    else
                        txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(1, "Changes updated successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-RewardMoney");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }

        }
        #endregion

        #region Reward Settings
        public IActionResult RewardSettings()
        {
            return View();
        }

        public IActionResult GetRewardMonthDaysList(JQueryDataTableParamModel param)
        {
            try
            {
                var allList = _rewardMonthDays.GetAll().Select(x => new RewardMonthDays
                {
                    Id = x.Id,
                    NoOfDays = x.NoOfDays,
                }).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    allList = allList.Where(x => x.NoOfDays.ToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                }

                var display = allList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var total = allList.Count;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = display
                });

            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetRewardMonthDaysList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditRewardMonthDays(int id)
        {
            if (id == 0) return View(@"Components/_AddEditRewardSettings", new RewardMonthDaysDto { Id = id });
            var result = _rewardMonthDays.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditRewardSettings", new RewardMonthDaysDto { Id = result.Id, NoOfDays = result.NoOfDays });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditRewardMonthDays(RewardMonthDaysDto model)
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

                    if (model.Id == 0)
                    {
                        var rewardMonthDays = new RewardMonthDays { NoOfDays = model.NoOfDays };

                        await _rewardMonthDays.InsertAsync(rewardMonthDays, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Reward month days inserted successfully.", rewardMonthDays.Id);
                    }
                    else
                    {
                        //var isExist = _rewardMonthDays.GetCount(x => x.DosageForm.ToLower().Equals(model.DosageForm.ToLower()) && x.Id != model.Id);
                        //if (isExist > 0)
                        //{
                        //    txscope.Dispose();
                        //    return JsonResponse.GenerateJsonResult(0, "Dosage Form already exists.");
                        //}
                        var result = _rewardMonthDays.GetSingle(x => x.Id == model.Id);
                        result.NoOfDays = model.NoOfDays;
                        await _rewardMonthDays.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Reward month days updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditRewardMonthDays");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        //[HttpPost]
        //public IActionResult RemoveRewardMonthDays(int id)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var result = _rewardMonthDays.GetSingle(x => x.Id == id);
        //            _rewardMonthDays.Delete(result);
        //            _rewardMonthDays.Save();
        //            txscope.Complete();
        //            return JsonResponse.GenerateJsonResult(1, @"Reward month days deleted successfully.");
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            ErrorLog.AddErrorLog(ex, "Post/RemoveRewardMonthDays");
        //            return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
        //        }
        //    }
        //}

        #endregion

        #region Money Range
        public IActionResult MoneyRange()
        {
            return View();
        }

        public async Task<IActionResult> GetMoneyRangeList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _rewardMoney.GetMoneyRangeList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetMoneyRangeList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

       
        [Route("/Admin/Reward/AddEditMoneyRange/{id}/{isEdit}")]
        public IActionResult AddEditMoneyRange(long id,bool isEdit)
        {
            //var isExistReferral = _rewardMoney.GetCount(x => x.RewardTypeId == (long)GlobalEnums.RewardType.Referral) != 0;
            //if (isExistReferral)
            //{
            //    ViewBag.RewardTypeList = _rewardTypeMaster.GetAll(x => x.Id != (long)GlobalEnums.RewardType.Referral && x.IsActive).Select(x => new SelectListItem
            //    {
            //        Text = x.Type,
            //        Value = x.Id.ToString()
            //    }).OrderBy(x => x.Text).ToList();
            //}

            if (isEdit)
            {
                ViewBag.RewardTypeList = _rewardMoney.GetAll(x => x.Id == id && x.IsActive).Select(x => new SelectListItem
                {
                    Text = x.RewardTypeMaster.Type,
                    Value = x.RewardTypeMaster.Id.ToString()
                }).OrderBy(x => x.Text).ToList();
            }
            else
            {
                ViewBag.RewardTypeList = _rewardTypeMaster.GetAll(x => x.IsActive).Select(x => new SelectListItem
                {
                    Text = x.Type,
                    Value = x.Id.ToString(),
                    Selected = true
                }).OrderBy(x => x.Text).ToList();
            }

            if (id == 0) return View(@"Components/_AddEditMoneyRange", new RewardMoneyDto { Id = id });
            var result = Mapper.Map<RewardMoneyDto>(_rewardMoney.GetSingle(x => x.Id == id));
            return View(@"Components/_AddEditMoneyRange", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditMoneyRange(RewardMoneyDto model)
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

                    int isExist;
                    if (model.Id == 0)
                    {
                        isExist = _rewardMoney.GetCount(x => x.MinRange == model.MinRange && x.MaxRange == model.MaxRange);
                        if (isExist != 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Money Range already exists !");
                        }
                        var rewardMoney = Mapper.Map<RewardMoneyDto, RewardMoneyMaster>(model);
                        rewardMoney.IsActive = true;
                        await _rewardMoney.InsertAsync(rewardMoney, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Money Range inserted successfully.", rewardMoney.Id);
                    }
                    isExist = _rewardMoney.GetCount(x => x.Id != model.Id && x.MinRange == model.MinRange && x.MaxRange == model.MaxRange);
                    if (isExist != 0)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Money Range already exists !");
                    }
                    var result = _rewardMoney.GetSingle(x => x.IsActive && x.Id == model.Id);
                    result.MinRange = model.MinRange;
                    result.MaxRange = model.MaxRange;
                    result.Description = model.Description;
                    result.RewardTypeId = model.RewardTypeId;
                    await _rewardMoney.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Money Range updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditMoneyRange");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMoneyRangeStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _rewardMoney.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _rewardMoney.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Money Range {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageMoneyRangeStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveMoneyRange(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _rewardMoney.GetSingle(x => x.Id == id);
                    _rewardMoney.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Money Range deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveMoneyRange");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Reward Product
        public IActionResult Product()
        {
            return View();
        }

        public async Task<IActionResult> GetProductList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _rewardProduct.GetRewardProductListAsync(parameters.ToArray());
                allList.ForEach(x => { x.ProductImage = $@"{FilePathList.RewardProduct}\{x.ProductImage}"; });

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
                ErrorLog.AddErrorLog(ex, "GetProductList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditRewardProduct(long id)
        {
            if (id == 0) return View(new RewardProductDto { Id = id });
            var result = Mapper.Map<RewardProductDto>(_rewardProduct.GetSingle(x => x.Id == id));
            result.ProductImage = $@"\{FilePathList.RewardProduct}\{result.ProductImage}";
            return View(result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditRewardProduct(RewardProductDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string newRewardProductFile = string.Empty;
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    if (model.ProductImageFile != null)
                    {
                        newRewardProductFile = CommonMethod.GetFileName(model.ProductImageFile.FileName);
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.RewardProduct, newRewardProductFile, model.ProductImageFile);
                    }
                    if (model.Id == 0)
                    {
                        var isExist = _rewardProduct.GetCount(x => x.ProductName.ToLower().Equals(model.ProductName.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Product Name already exists !");
                        }

                        var rewardproduct = Mapper.Map<RewardProductDto, RewardProduct>(model);
                        rewardproduct.ProductImage = newRewardProductFile;
                        rewardproduct.IsActive = true;
                        await _rewardProduct.InsertAsync(rewardproduct, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Reward product inserted successfully.", rewardproduct.Id);
                    }
                    else
                    {

                        var isExist = _rewardProduct.GetCount(x => x.ProductName.ToLower().Equals(model.ProductName.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Product already exists !");
                        }

                        var result = _rewardProduct.GetSingle(x => x.Id == model.Id);
                        var oldRewardProductFile = result.ProductImage;
                        result.ProductName = model.ProductName;
                        result.Redeem = model.Redeem;
                        result.Description = model.Description;
                        result.ProductImage = !string.IsNullOrEmpty(newRewardProductFile) ? newRewardProductFile : oldRewardProductFile;
                        await _rewardProduct.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        if (!string.IsNullOrEmpty(newRewardProductFile) && !string.IsNullOrEmpty(oldRewardProductFile))
                        {
                            CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Testimonial, oldRewardProductFile), true);
                        }
                        return JsonResponse.GenerateJsonResult(1, "Reward product updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    if (!string.IsNullOrEmpty(newRewardProductFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Testimonial, newRewardProductFile), true);
                    }
                    ErrorLog.AddErrorLog(ex, "Post/AddEditRewardProduct");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageRewardProductstatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _rewardProduct.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _rewardProduct.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Reward product {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageRewardProductstatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveRewardProduct(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _rewardProduct.GetSingle(x => x.Id == id);
                    _rewardProduct.Delete(result);
                    txscope.Complete();
                    CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.RewardProduct, result.ProductImage), true);
                    return JsonResponse.GenerateJsonResult(1, @"Reward product deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveRewardProduct");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Reward Earn History
        public IActionResult MoneyHistory()
        {
            return View();
        }
        public async Task<IActionResult> GetRewardEarnHistoryList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = 0 });
                var allList = await _rewardEarn.GetRewardEarnHistoryList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetRewardEarnHistoryList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult EarnMoney(long id)
        {
            return View();
        }
        public async Task<IActionResult> GetEarnMoneyList(long id, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = id });
                var allList = await _rewardEarn.GetEarnMoneyList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetEarnMoneyList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public async Task<IActionResult> RedeemProduct(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = id }
            };
            var result = await _rewardEarn.GetEarnedRewardProductList(parameters.ToArray());
            return View(result);
        }
        #endregion

        #region Redeem Request
        public IActionResult RedeemRequest()
        {
            return View();
        }
        public async Task<IActionResult> GetRedeemRequestList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _redeemRequest.GetRedeemRequestList(parameters.ToArray());
                allList.ForEach(x => { x.ProductImage = $@"{FilePathList.RewardProduct}\{x.ProductImage}"; });
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
                ErrorLog.AddErrorLog(ex, "GetRedeemRequestList");
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

        #endregion

        #region Controller Common
        [HttpPost]
        public async Task<IActionResult> RedeemRequestStatus(long id, bool isApprove)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _redeemRequest.GetSingle(x => x.Id == id);
                    if (isApprove)
                    {
                        var parameters = new List<SqlParameter>
                        {
                           new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = result.PharmacyId },
                           new SqlParameter("@IsAdmin", SqlDbType.Bit) { Value = true }
                        };
                        var available = await _rewardEarn.GetRewardEarnProducts(parameters.ToArray());
                        if (available.AvailableReward.AvailableRewardMoney < result.RewardProduct.Redeem)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(1, $@"No reward available !", result.Id);
                        }
                    }

                    result.IsApprove = isApprove;
                    await _redeemRequest.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    await SendEmail(id, isApprove);
                    return JsonResponse.GenerateJsonResult(1, $@"Redeem request {(isApprove ? "approved" : "rejected")} successfully.", result.Id);

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RedeemRequestStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDeliveryStatus(long id, short status)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _redeemRequest.GetSingle(x => x.Id == id);
                    result.DeliveryStatus = status;
                    await _redeemRequest.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Delivery status changed successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ChangeDeliveryStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public async Task SendEmail(long Id, bool Status)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var pharmacyObj = _redeemRequest.GetById(Id).Pharmacy;
                    string physicalUrl = GetPhysicalUrl();
                    string subject = "Redeem Request";
                    string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.PharmacyRedeemRequest, physicalUrl);
                    emailTemplate = emailTemplate.Replace("{UserName}", pharmacyObj.PharmacyAdminUser.FullName);
                    emailTemplate = emailTemplate.Replace("{RedeemMessage}", $@"Redeem Request {(Status ? "approved" : "rejected")} successfully.");
                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                    {
                        ToAddress = pharmacyObj.PharmacyAdminUser.Email,
                        Subject = $@"Pharmacy {subject}",
                        BodyText = emailTemplate
                    });
                    txscope.Complete();
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "RedeemRewardRequest/SendEmail");
                }
            }
        }
        #endregion
    }
}