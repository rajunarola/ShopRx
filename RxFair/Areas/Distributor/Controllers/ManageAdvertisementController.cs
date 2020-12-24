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
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class ManageAdvertisementController : BaseController<ManageAdvertisementController>
    {
        #region Fields
        private readonly IAdvertisementService _advertisement;
        private readonly IAdvertisementMedicineService _advertisementMedicine;
        private readonly IDosageFormService _dosageForm;
        private readonly IMedicineMasterService _medicine;
        #endregion

        #region ctor
        public ManageAdvertisementController(IAdvertisementService advertisement, IAdvertisementMedicineService advertisementMedicine, IDosageFormService dosageForm, IMedicineMasterService medicine)
        {
            _advertisement = advertisement;
            _advertisementMedicine = advertisementMedicine;
            _dosageForm = dosageForm;
            _medicine = medicine;
        }

        #endregion

        #region Method

        [HttpGet]
        [Route("Distributor/ManageAdvertisement/TopDeals")]
        [Route("Distributor/ManageAdvertisement/DealOfTheDay")]
        [Route("Distributor/ManageAdvertisement/PriceIncrease")]
        public ActionResult ManageRequest()
        {
            var route = HttpContext.Request.Path.Value.Split("/");
            ViewBag.RequestType = route[3]; 

            switch (route[3])
            {
                case DealType.TopDeals:
                    ViewBag.DealType = (short)GlobalEnums.DealType.TopDeals;
                    ViewBag.RequestLabel = DealTypeLabels.TopDeals;
                    break;
                case DealType.DealOfTheDay:
                    ViewBag.DealType = (short)GlobalEnums.DealType.DealOfTheDay;
                    ViewBag.RequestLabel = DealTypeLabels.DealOfTheDay;
                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    var subscriptionTypeId = Convert.ToInt64(User.GetClaimValue(UserClaims.SubscriptionTypeId));
                    if (subscriptionTypeId == (long)GlobalEnums.SubscriptionTypes.Gold)
                    {
                        var advertisementCount = _advertisement.GetCount(x => x.DealType == (short)GlobalEnums.DealType.DealOfTheDay
                                                                              && x.DistributorId == distributorId && (x.CreatedDate.Month == DateTime.Today.Month
                                                                                                                      && x.CreatedDate.Year == DateTime.Today.Year));
                        if (advertisementCount >= (int)GlobalEnums.AdvertisementLimit.GoldSubscriptionLimit)
                        {
                            TempData["ErrorMessage"] = GlobalConstant.GoldSubscriptionLimit;
                            ViewBag.IsExpire = true;
                        }
                    }
                    break;
                case DealType.ProductPriceIncrease:
                    ViewBag.DealType = (short)GlobalEnums.DealType.ProductPriceIncrease;
                    ViewBag.RequestLabel = DealTypeLabels.ProductPriceIncrease;
                    break;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvertiseMentList(JQueryDataTableParamModel param, int dealType, bool? status)
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                parameters.Insert(1, new SqlParameter("@DealType", SqlDbType.Int) { Value = dealType });
                parameters.Insert(2, new SqlParameter("@Status", SqlDbType.Bit) { Value = status });
                var allList = await _advertisement.GetAdvertisementList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetAdvertiseMentList");
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
        [Route("/Distributor/ManageAdvertisement/{requestType}/{isView}/{id?}")]
        public async Task<IActionResult> CreateRequest(string requestType, string isView, long? id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    ViewBag.IsView = (isView == "View");
                    ViewBag.RequestType = requestType;
                    #region CheckSubscription

                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    var subscriptionTypeId = Convert.ToInt64(User.GetClaimValue(UserClaims.SubscriptionTypeId));
                    if (isView == "Add")
                    {
                        if (subscriptionTypeId == (long)GlobalEnums.SubscriptionTypes.Gold)
                        {
                            var advertisementCount = _advertisement.GetCount(x => x.DealType == (short)GlobalEnums.DealType.DealOfTheDay
                                                     && x.DistributorId == distributorId && (x.CreatedDate.Month == DateTime.Today.Month
                                                     && x.CreatedDate.Year == DateTime.Today.Year));
                            if (advertisementCount >= (int)GlobalEnums.AdvertisementLimit.GoldSubscriptionLimit)
                            {
                                TempData["ErrorMessage"] = GlobalConstant.GoldSubscriptionLimit;
                                return Redirect("Distributor/ManageAdvertisement/" + requestType);
                            }
                        }
                    }
                    #endregion

                    switch (requestType)
                    {
                        case DealType.TopDeals:
                            ViewBag.DealType = (short)GlobalEnums.DealType.TopDeals;
                            ViewBag.RequestLabel = DealTypeLabels.TopDeals;
                            break;
                        case DealType.DealOfTheDay:
                            ViewBag.DealType = (short)GlobalEnums.DealType.DealOfTheDay;
                            ViewBag.RequestLabel = DealTypeLabels.DealOfTheDay;
                            break;
                        case DealType.ProductPriceIncrease:
                            ViewBag.DealType = (short)GlobalEnums.DealType.ProductPriceIncrease;
                            ViewBag.RequestLabel = DealTypeLabels.ProductPriceIncrease;
                            break;
                    }

                    if (id == null) return View();
                    {
                        var advertisement = _advertisement.GetSingle(x => x.Id == id);

                        AdvertisementDto advertisementDto = new AdvertisementDto();
                        switch (advertisement.DealType)
                        {
                            case (short)GlobalEnums.DealType.TopDeals:
                                advertisementDto.AdvStartDate = advertisement.StartDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
                                advertisementDto.AdvEndDate = advertisement.EndDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
                                break;
                            case (short)GlobalEnums.DealType.DealOfTheDay:
                                advertisementDto.AdvDealDate = advertisement.DealDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
                                break;
                            case (short)GlobalEnums.DealType.ProductPriceIncrease:
                                advertisementDto.AdvPriceIncreaseDate = advertisement.PriceIncreaseDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
                                break;
                        }
                        advertisementDto.Id = advertisement.Id;
                        advertisementDto.Request = advertisement.Request;

                        SqlParameter[] parameter = { new SqlParameter("@AdvertisementId", SqlDbType.BigInt) { Value = advertisementDto.Id } };
                        var advertisementMedicineList = await _advertisement.GetAdvertisementMedicineList(parameter);
                        List<MedicineDto> medicineList = new List<MedicineDto>();
                        medicineList.AddRange(advertisementMedicineList);
                        advertisementDto.Medicine = medicineList;
                        return View(advertisementDto);
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/CreateRequest");
                    return View();
                }
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetMedicineList(long? advertisementId, int dealType, string medicineIdList, JQueryDataTableParamModel param)
        {
            try
            {
                var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                parameters.Insert(1, new SqlParameter("@DealType", SqlDbType.Int) { Value = dealType });
                parameters.Insert(2, new SqlParameter("@AdvertisementId", SqlDbType.BigInt) { Value = advertisementId });
                parameters.Insert(3, new SqlParameter("@MedicineIdList", SqlDbType.NVarChar) { Value = medicineIdList });
                var allList = await _advertisement.GetMedicineList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetMedicineList");
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
        public async Task<IActionResult> AddEditAdvertisement(AdvertisementDto model)
        {

            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        model.DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                        Advertisement advertisement = new Advertisement
                        {
                            IsActive = true,
                            Request = model.Request,
                            DistributorId = model.DistributorId,
                            DealType = model.DealType,
                            Status = model.Status,
                            StartDate = Convert.ToDateTime(model.AdvStartDate),
                            EndDate = Convert.ToDateTime(model.AdvEndDate),
                            DealDate = Convert.ToDateTime(model.AdvDealDate),
                            PriceIncreaseDate = Convert.ToDateTime(model.AdvPriceIncreaseDate),
                        };

                        var advertisementResult = await _advertisement.InsertAsync(advertisement, Accessor, User.GetUserId());
                        foreach (var y in model.Medicine)
                        {
                            //var medicineId = _medicine.GetSingle(x => x.NdcUpcHri == y.NDC).Id;
                            AdvertisementMedicine advertisementMedicine = new AdvertisementMedicine
                            {
                                AdvertisementId = advertisementResult.Id,
                                MedicineId = y.Id,
                                DealPrice = y.DealPrice
                            };
                            await _advertisementMedicine.InsertAsync(advertisementMedicine, Accessor, User.GetUserId());

                        }
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.RequestAdverisement);
                    }
                    else
                    {
                        //Update Code
                        var advertisement = _advertisement.GetSingle(x => x.Id == model.Id);
                        advertisement.Request = model.Request;
                        advertisement.StartDate = Convert.ToDateTime(model.AdvStartDate);
                        advertisement.EndDate = Convert.ToDateTime(model.AdvEndDate);
                        advertisement.DealDate = Convert.ToDateTime(model.AdvDealDate);

                        advertisement.PriceIncreaseDate = Convert.ToDateTime(model.AdvPriceIncreaseDate);
                        await _advertisement.UpdateAsync(advertisement, Accessor, User.GetUserId());

                        var modelMed = model.Medicine.Select(x => x.Id).ToList();
                        var existingAdvMedList = advertisement.AdvertisementMedicines.ToList();
                        var existMedicine = existingAdvMedList.Select(x => x.MedicineId).ToList();

                        #region Remove Medicine
                        var removeMedicine = existMedicine.Except(modelMed).ToList();
                        if (removeMedicine.Any())
                        {
                            var removeAdvertisement = existingAdvMedList.Where(x => removeMedicine.Contains(x.MedicineId)).ToList();
                            _advertisementMedicine.DeleteRange(removeAdvertisement, Accessor, User.GetUserId());
                            await _advertisementMedicine.SaveAsync();
                        }
                        #endregion

                        #region Insert New Medicine
                        var addMedicineList = modelMed.Except(existMedicine).ToList();
                        var newMedicineList = new List<AdvertisementMedicine>();
                        foreach (var item in addMedicineList)
                        {
                            newMedicineList.Add(new AdvertisementMedicine
                            {
                                AdvertisementId = model.Id,
                                IsActive = true,
                                MedicineId = item,
                                DealPrice = model.Medicine.FirstOrDefault(x => x.Id == item)?.DealPrice ?? 0
                            });
                        }
                        if (newMedicineList.Any())
                        {
                            await _advertisementMedicine.InsertRangeAsync(newMedicineList, Accessor, User.GetUserId());
                        }
                        #endregion

                        #region Edit Medicine
                        foreach (var item in existMedicine.Where(x => !removeMedicine.Contains(x)))
                        {
                            var editAdvMed = existingAdvMedList.SingleOrDefault(x => x.AdvertisementId == model.Id && x.MedicineId == item);
                            if (editAdvMed == null) continue;
                            {
                                editAdvMed.DealPrice =
                                    model.Medicine.SingleOrDefault(x => x.Id == item)?.DealPrice ?? 0;
                                await _advertisementMedicine.UpdateAsync(editAdvMed, Accessor, User.GetUserId());
                            }
                        }
                        #endregion

                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateAdvertisement);
                    }


                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditAdvertisement");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }


        public async Task<IActionResult> RemoveAdvertisementMedicine(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //Removing AdvertisementRequest from AdvertisementMedicine
                    var advertisementMedicines = _advertisementMedicine.GetAll(x => x.AdvertisementId == id);
                    _advertisementMedicine.DeleteRange(advertisementMedicines, Accessor, User.GetUserId());
                    await _advertisementMedicine.SaveAsync();

                    //Removing AdvertisementRequest from Advertisement
                    var advertisement = _advertisement.GetSingle(x => x.Id == id);
                    await _advertisement.DeleteAsync(advertisement, Accessor, User.GetUserId());
                    await _advertisement.SaveAsync();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.DeleteAdvertisement);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveAdvertisementMedicine");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageIsActive(long id)
        {
            try
            {
                var advertisement = _advertisement.GetSingle(x => x.Id == id);
                advertisement.IsActive = !advertisement.IsActive;
                await _advertisement.UpdateAsync(advertisement, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Advertisement {(advertisement.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-ManageAdvertisement/ManageIsActive");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }
        #endregion

        #region Common

        [HttpGet]
        public IActionResult CheckSubscription()
        {
            #region CheckSubscription
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            var subscriptionTypeId = Convert.ToInt64(User.GetClaimValue(UserClaims.SubscriptionTypeId));
            if (subscriptionTypeId != (long)GlobalEnums.SubscriptionTypes.Gold)
                return JsonResponse.GenerateJsonResult(1, GlobalConstant.GoldSubscriptionLimit);
            var advertisementCount = _advertisement.GetCount(x => x.DealType == (short)GlobalEnums.DealType.DealOfTheDay
                                                                  && x.DistributorId == distributorId && (x.CreatedDate.Month == DateTime.Today.Month
                                                                  && x.CreatedDate.Year == DateTime.Today.Year));

            return JsonResponse.GenerateJsonResult(
                advertisementCount >= (int)GlobalEnums.AdvertisementLimit.GoldSubscriptionLimit ? 0 : 1,
                GlobalConstant.GoldSubscriptionLimit);
            #endregion
        }

        #endregion
    }
}