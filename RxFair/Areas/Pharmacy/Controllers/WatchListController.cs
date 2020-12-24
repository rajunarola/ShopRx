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

namespace RxFair.Areas.Pharmacy.Controllers
{

    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class WatchListController : BaseController<WatchListController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IMedicinePriceMasterService _medicinePrice;
        private readonly IWatchlistService _watchlist;
        private readonly IManufacturerService _manufacturer;
        private readonly IMedicineMasterService _medicine;
        private readonly ICartService _cart;

        #endregion

        #region Ctor
        public WatchListController(IWatchlistService watchlist, IManufacturerService manufacturer, IMedicineMasterService medicine, ICartService cart, IDistributorService distributor, IMedicinePriceMasterService medicinePrice)
        {
            _distributor = distributor;
            _medicinePrice = medicinePrice;
            _watchlist = watchlist;
            _manufacturer = manufacturer;
            _medicine = medicine;
            _cart = cart;
        }

        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWatchList(JQueryDataTableParamModel param)
        {
            try
            {
                var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));

                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
                var allList = await _watchlist.GetWatchList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetWatchList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        //[HttpGet]
        //[Route("/Pharmacy/WatchList/AddWatchList/{distributorId}/{id}/{isView}/{isExist}")]
        //public async Task<IActionResult> AddWatchList(long distributorId, long id, bool isView, bool isExist)
        //{
        //    ViewBag.isView = isExist || isView;

        //    var result = await _watchlist.GetSingleAsync(x => x.MedicineId == id && x.DistributorId == distributorId);

        //    var model = new WatchListDto
        //    {
        //        DistributorName = result.Distributor.CompanyName ?? "",
        //        Price = result.MedicineMaster.MedicinePriceMasters.
        //                FirstOrDefault(x => x.DistributorId == distributorId)?.WacpackagePrice ?? 0,
        //        DistributorId = distributorId,
        //        MedicineId = id,
        //        Ndc = result.MedicineMaster.NdcUpcHri,
        //        Quantity = result?.Quantity ?? 1,
        //        MatchPrice = result?.MatchPrice ?? 1,
        //        MedicineName = result.MedicineMaster.DrugName,
        //        Manufacturer = result.MedicineMaster?.ManufacturerMaster?.ManufacturerName ?? "",
        //        Strength = result.MedicineMaster.Strength,
        //        IsExist = isExist,
        //        Category = result.MedicineMaster.CategoryMaster?.MedicineCategory ?? "",
        //    };
        //    return View(@"Components/_AddWatchList", model);
        //}
        [HttpGet]
        [Route("/Pharmacy/WatchList/AddWatchList/{distributorId}/{id}/{isView}/{isExist}")]
        public async Task<IActionResult> AddWatchList(long distributorId, long id, bool isView, bool isExist)
        {
            ViewBag.isView = isExist || isView;

            var distributorName = await _distributor.GetSingleAsync(x => x.Id == distributorId);
            var price = await _medicinePrice.GetSingleAsync(x => x.MedicineId == id && x.DistributorId == distributorId);
            var medicine = await _medicine.GetSingleAsync(x => x.Id == id);
            var manName = _manufacturer.GetSingle(x => x.Id == medicine.ManufacturerId).ManufacturerName;
            var result = _watchlist.FindBy(x => x.MedicineId == id && x.DistributorId == distributorId).FirstOrDefault();

            var model = new WatchListDto
            {
                Id = result?.Id ?? 0,
                DistributorName = distributorName.CompanyName,
                Price = price?.WacpackagePrice ?? 0,
                DistributorId = distributorId,
                MedicineId = id,
                Ndc = medicine.NdcUpcHri,
                Quantity = result?.Quantity ?? 1,
                MatchPrice = result?.MatchPrice ?? 1,
                MedicineName = medicine.DrugName,
                Manufacturer = manName,
                Strength = medicine.Strength,
                IsExist = isExist,
                Category = "Generic"
            };
            return View(@"Components/_AddWatchList", model);
        }

        //On Find click
        [HttpPost]
        public async Task<IActionResult> AddWatchList(string search)
        {
            try
            {
                var parameters = new List<SqlParameter> { new SqlParameter("@Search", SqlDbType.NVarChar) { Value = search } };
                var result = await _watchlist.GetWatchListDetails(parameters.ToArray());
                var temp = new WatchListDto
                {
                    Id = result.Id,
                    MedicineName = result.MedicineName,
                    Manufacturer = result.Manufacturer,
                    Strength = result.Strength,
                    Category = result.Category
                };
                return Json(temp);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post/AddWatchList");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        //Insert into watchlist
        [HttpPost]
        public async Task<IActionResult> InsertWatchList(WatchListDto model)
        {
            var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _watchlist.GetSingle(x => x.MedicineId == model.Id && x.DistributorId == model.DistributorId && x.PharmacyId == pharmacyId);
                    if (result != null)
                    {
                        result.MatchPrice = model.MatchPrice;
                        result.Quantity = model.Quantity;
                        result.IsNotified = false;

                        await _watchlist.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();

                        return JsonResponse.GenerateJsonResult(1, @"Watch List updated successfully.");
                    }

                    var list = new WatchList
                    {
                        IsActive = true,
                        DistributorId = model.DistributorId,
                        Quantity = model.Quantity,
                        MatchPrice = model.MatchPrice,
                        MedicineId = model.Id,
                        MedicineDate = Convert.ToDateTime(DateTime.Now.Date),
                        PharmacyId = pharmacyId,
                        IsNotified = false
                    };
                    await _watchlist.InsertAsync(list, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Watch List added successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/InsertWatchList");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveWatchList(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _watchlist.GetSingle(x => x.Id == id);
                    _watchlist.Delete(result);
                    _watchlist.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"WatchList deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveWatchList");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult GetListOnSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetWatchListOnSearch(string search, JQueryDataTableParamModel param)
        {
            try
            {
                var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));

                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@PharmacyId", SqlDbType.BigInt) { Value = pharmacyId });
                parameters.Insert(1, new SqlParameter("@SearchText", SqlDbType.VarChar) { Value = search });

                var allList = await _watchlist.GetWatchListOnSearch(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetWatchListOnSearch");
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
        public async Task<IActionResult> AddtoCart(CartDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    model.PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    model.CartDate = Convert.ToDateTime(DateTime.Now.Date);
                    var oldMedicine = await _cart.GetSingleAsync(x => x.MedicineId == model.MedicineId && x.DistributorId == model.DistributorId && x.PharmacyId == model.PharmacyId);
                    if (oldMedicine != null)
                    {
                        // UpdateCart
                        oldMedicine.CartDate = model.CartDate;
                        oldMedicine.Quantity = model.Quantity;
                        await _cart.UpdateAsync(oldMedicine, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.UpdateCart);
                    }

                    // Insert Cart
                    var cartObj = Mapper.Map<Cart>(model);
                    await _cart.InsertAsync(cartObj, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.InsertCart);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddtoCart");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion
    }
}