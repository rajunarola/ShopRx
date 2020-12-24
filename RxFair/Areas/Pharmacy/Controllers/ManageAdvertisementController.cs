using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class ManageAdvertisementController : BaseController<ManageAdvertisementController>
    {
        #region Fields
        private readonly IPharmacyService _pharmacy;
        private readonly ICartService _cart;
        #endregion

        #region ctor
        public ManageAdvertisementController(IPharmacyService pharmacy, ICartService cart)
        {
            _pharmacy = pharmacy;
            _cart = cart;
        }
        #endregion

        #region Methods

        [HttpGet]
        [Route("Pharmacy/ManageAdvertisement/TopDeals")]
        [Route("Pharmacy/ManageAdvertisement/DealOfTheDay")]
        [Route("Pharmacy/ManageAdvertisement/PriceIncrease")]
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
                    break;
                case DealType.ProductPriceIncrease:
                    ViewBag.DealType = (short)GlobalEnums.DealType.ProductPriceIncrease;
                    ViewBag.RequestLabel = DealTypeLabels.ProductPriceIncrease;
                    break;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDistributorAdvertisement(short dealType, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DealType", SqlDbType.Int) { Value = dealType });
                var allList = await _pharmacy.GetPharmacyAdvertisementList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetDistributorAdvertisement");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult ViewMedicines(long id, short dealType)
        {
            ViewBag.DistributorId = id;
            ViewBag.DealType = dealType;
            switch (dealType)
            {
                case (short)GlobalEnums.DealType.TopDeals:
                    ViewBag.RequestLabel = DealTypeLabels.TopDeals;
                    break;
                case (short)GlobalEnums.DealType.DealOfTheDay:
                    ViewBag.RequestLabel = DealTypeLabels.DealOfTheDay;
                    break;
                case (short)GlobalEnums.DealType.ProductPriceIncrease:
                    ViewBag.RequestLabel = DealTypeLabels.ProductPriceIncrease;
                    break;
            }
            return View(@"Components/_ViewMedicines");
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyMedicines(short dealType, long distributorId, JQueryDataTableParamModel param)
        {
            try
            {
                var PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId });
                parameters.Insert(1, new SqlParameter("@DealType", SqlDbType.Int) { Value = dealType });
                parameters.Insert(2, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = PharmacyId });
                var allList = await _pharmacy.GetPharmacyMedicinesList(parameters.ToArray());

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
                ErrorLog.AddErrorLog(ex, "GetPharmacyMedicines");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> AddtoCart(CartDto model)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            model.CartDate = Convert.ToDateTime(DateTime.Now.Date);
        //            model.PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
        //            var OldMedicine = await _cart.GetSingleAsync(x => x.MedicineId == model.MedicineId);
        //            if (OldMedicine != null)
        //            {
        //                // UpdateCart
        //                OldMedicine.CartDate = model.CartDate;
        //                OldMedicine.Quantity = model.Quantity;
        //                await _cart.UpdateAsync(OldMedicine, Accessor, User.GetUserId());
        //                txscope.Complete();
        //                return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateCart);
        //            }

        //                // Insert Cart
        //                var CartObj = Mapper.Map<Cart>(model);
        //                await _cart.InsertAsync(CartObj, Accessor, User.GetUserId());
        //                txscope.Complete();
        //                return JsonResponse.GenerateJsonResult(1, GlobalConstant.InsertCart);
        //        }
        //        catch (Exception ex)
        //        {
        //            txscope.Dispose();
        //            ErrorLog.AddErrorLog(ex, "Post/AddtoCart");
        //            return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
        //        }
        //    }
        //}

        #endregion

        #region Common
        #endregion

    }
}