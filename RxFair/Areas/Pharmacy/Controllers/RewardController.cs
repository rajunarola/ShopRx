using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Data.DbModel;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class RewardController : BaseController<RewardController>
    {
        private readonly IRewardEarnService _rewardEarn;
        private readonly IRedeemRequestService _redeemRequest;
        private readonly IRewardProductService _rewardProduct;

        #region Ctor
        public RewardController(IRewardEarnService rewardEarn, IRedeemRequestService redeemRequest, IRewardProductService rewardProduct)
        {
            _rewardProduct = rewardProduct;
            _rewardEarn = rewardEarn;
            _redeemRequest = redeemRequest;
        }
        #endregion

        #region Methods
        public async Task<IActionResult> RedeemMoney()
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = User.GetClaimValue(UserClaims.PharmacyId) },
                new SqlParameter("@IsAdmin", SqlDbType.Bit) { Value = false }
            };
            var result = await _rewardEarn.GetRewardEarnProducts(parameters.ToArray());
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> RedeemRequest(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    var TotalReward = _rewardEarn.GetAll(x => x.PharmacyId == pharmacyId).GroupBy(x => x.PharmacyId).Select(x => x.Sum(y => y.RewardMoney));
                    var availableReward = _redeemRequest.GetAll(x => x.PharmacyId == pharmacyId).Select(x => x.RewardProduct).Select(x => x.Redeem).Sum();
                    var productAmount = _rewardProduct.GetById(id).Redeem;

                    //if avalible reward amount is Lesser than redeem product amount 
                    if ((TotalReward.FirstOrDefault()- availableReward) < productAmount)
                    {
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, @"Insufficient Money.");
                    }
                    else
                    {
                        var model = new RedeemRequest
                        {
                            RewardProductId = id,
                            PharmacyId = pharmacyId,
                            IsApprove = null,
                            DeliveryStatus = (short)GlobalEnums.DeliveryType.Pending
                        };
                        await _redeemRequest.InsertAsync(model, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, @"Redeem request sent successfully.");
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RedeemRequest");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult MoneyHistory()
        {
            return View();
        }

        public async Task<IActionResult> GetEarnMoneyList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId)) });
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
                ErrorLog.AddErrorLog(ex, "Pharmacy-GetEarnMoneyList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public async Task<IActionResult> RedeemProduct()
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = User.GetClaimValue(UserClaims.PharmacyId) }
            };
            var result = await _rewardEarn.GetEarnedRewardProductList(parameters.ToArray());
            return View(result);
        }

        public async Task<IActionResult> RebateProgress() {

            try { 

                    var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = User.GetClaimValue(UserClaims.PharmacyId) }
                    };
                    var result = await _rewardEarn.GetRebateProgressInfo(parameters.ToArray());
                    return View(result);

                }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Pharmacy-RebateProgress");
                return View();
            }
        }

        #endregion

        #region Controller Common

        #endregion

    }
}