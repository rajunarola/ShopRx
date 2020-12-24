using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using RxFair.Dto.Enum;


namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class PharmacyProfileController : BaseController<PharmacyProfileController>
    {
        #region Fields
        private readonly IPharmacyService _pharmacy;
        private readonly IStateService _state;
        private readonly IPharmacyBillingAddressService _pharmacyBilling;
        private readonly IPharmacyShippingAddressService _pharmacyShipping;
        private readonly IPharmacySystemMasterService _pharmacySystem;
        private readonly IPharmacyTypeMasterService _pharmacyTypeMaster;
        private readonly IUserService _user;
        #endregion

        #region ctor
        public PharmacyProfileController(IPharmacyService pharmacy, IStateService state, IPharmacyBillingAddressService pharmacyBillingAddress, IPharmacyShippingAddressService pharmacyShippingAddress, IPharmacySystemMasterService pharmacySystemMaster, IPharmacyTypeMasterService typeMaster, IUserService user)
        {
            _pharmacy = pharmacy;
            _state = state;
            _pharmacyBilling = pharmacyBillingAddress;
            _pharmacyShipping = pharmacyShippingAddress;
            _pharmacySystem = pharmacySystemMaster;
            _pharmacyTypeMaster = typeMaster;
            _user = user;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            try
            {
                BindDropdownList();
                var pharmacy = _pharmacy.GetSingle(x => x.UserId == User.GetUserId());
                var model = new ViewPharmacyProfileDto
                {
                    Pharmacy = Mapper.Map<NewPharmacyDto>(pharmacy),
                    PharmacyBillAddresses = Mapper.Map<List<PharmacyBillOrShipAddressDto>>(pharmacy.BillingAddresses),
                    PharmacyShipAddresses = Mapper.Map<List<PharmacyBillOrShipAddressDto>>(pharmacy.ShippingAddresses),
                };
                model.Pharmacy.FirstName = pharmacy.PharmacyAdminUser.FirstName;
                model.Pharmacy.LastName = pharmacy.PharmacyAdminUser.LastName;
                model.Pharmacy.JobTitle = pharmacy.PharmacyAdminUser.JobTitle;
                model.Pharmacy.PrimaryEmail = pharmacy.PharmacyAdminUser.Email;
                model.Pharmacy.PhoneNumber = pharmacy.PharmacyAdminUser.PhoneNumber;
                model.Pharmacy.MobileNumber = pharmacy.PharmacyAdminUser.Mobile;
                model.Pharmacy.LicenseExpiresDate = pharmacy.LicenseExpires.ToDefaultDateTime("MMM-dd-yyyy");
                model.Pharmacy.DeaExpriesDate = pharmacy.DeaExpires.ToDefaultDateTime("MMM-dd-yyyy");
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "PharmacyProfile-Index");
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [Route("AddEditBillingOrShippingAddress/{id}/{isBilling}/{pharmacyId}")]
        public IActionResult AddEditBillingOrShippingAddress(long id, bool isBilling, long pharmacyId)
        {
            BindStateList();
            if (id == 0) return View(@"Components/_AddEditAddress", new PharmacyBillOrShipAddressDto { Id = id, IsBilling = isBilling, PharmacyId = pharmacyId });
            var result = isBilling
                        ? Mapper.Map<PharmacyBillOrShipAddressDto>(_pharmacyBilling.GetById(id))
                        : Mapper.Map<PharmacyBillOrShipAddressDto>(_pharmacyShipping.GetById(id));
            result.IsBilling = isBilling;
            return View(@"Components/_AddEditAddress", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditBillingOrShippingAddress(PharmacyBillOrShipAddressDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (model.Id == 0)
                    {
                        ResetDefaultAddress(model.IsBilling, model.PharmacyId);
                        if (model.IsBilling)
                        {
                            var pharmacyBillingAddress = Mapper.Map<PharmacyBillOrShipAddressDto, PharmacyBillingAddress>(model);
                            pharmacyBillingAddress.IsActive = true;
                            pharmacyBillingAddress.IsDefault = true;
                            await _pharmacyBilling.InsertAsync(pharmacyBillingAddress, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, GlobalConstant.BillingAddressUpdated);
                        }
                        var pharmacyShippingAddress = Mapper.Map<PharmacyBillOrShipAddressDto, PharmacyShippingAddress>(model);
                        pharmacyShippingAddress.IsActive = true;
                        pharmacyShippingAddress.IsDefault = true;
                        await _pharmacyShipping.InsertAsync(pharmacyShippingAddress, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.ShippingAddressUpdated);
                    }

                    if (model.IsBilling)
                    {
                        var pharmacyBill = _pharmacyBilling.GetSingle(x => x.Id == model.Id);

                        pharmacyBill.Address1 = model.Address1;
                        pharmacyBill.Address2 = model.Address2;
                        pharmacyBill.City = model.City;
                        pharmacyBill.StateId = model.StateId;
                        pharmacyBill.ZipCode = model.ZipCode;
                        pharmacyBill.PharmacyId = model.PharmacyId;

                        await _pharmacyBilling.UpdateAsync(pharmacyBill, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.BillingAddressUpdated);
                    }
                    var pharmacyShip = _pharmacyShipping.GetSingle(x => x.Id == model.Id);

                    pharmacyShip.Address1 = model.Address1;
                    pharmacyShip.Address2 = model.Address2;
                    pharmacyShip.City = model.City;
                    pharmacyShip.StateId = model.StateId;
                    pharmacyShip.ZipCode = model.ZipCode;
                    pharmacyShip.PharmacyId = model.PharmacyId;

                    await _pharmacyShipping.UpdateAsync(pharmacyShip, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.ShippingAddressUpdated);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditBillingOrShippingAddress");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemovePharmacyAddress(long id, bool isBilling)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (isBilling)
                    {
                        var result = await _pharmacyBilling.GetSingleAsync(x => x.Id == id);
                        await _pharmacyBilling.DeleteAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.BillingAddressDeleted);
                    }
                    else
                    {
                        var result = await _pharmacyShipping.GetSingleAsync(x => x.Id == id);
                        await _pharmacyShipping.DeleteAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.ShippingAddressDeleted);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemovePharmacyAddress");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDefaultAddress(long id, bool isBilling)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                    if (isBilling)
                    {
                        ResetDefaultAddress(true, pharmacyId);
                        var result = _pharmacyBilling.GetById(id);
                        result.IsDefault = true;
                        await _pharmacyBilling.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Default address changed.");
                    }
                    else
                    {
                        ResetDefaultAddress(false, pharmacyId);
                        var result = _pharmacyShipping.GetById(id);
                        result.IsDefault = true;
                        await _pharmacyShipping.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Default address changed.");
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemovePharmacyAddress");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePharmacy(ViewPharmacyProfileDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacy = await _pharmacy.GetSingleAsync(x => x.Id == model.Pharmacy.Id);
                    //var PharmacyUserInfo = await _user.GetSingleAsync(x => x.Id == model.Pharmacy.UserId);

                    pharmacy.LicenseNumber = model.Pharmacy.LicenseNumber;
                    pharmacy.DeaNumber = model.Pharmacy.DeaNumber;
                    pharmacy.DeaExpires = Convert.ToDateTime(model.Pharmacy.DeaExpriesDate);
                    pharmacy.LicenseExpires = Convert.ToDateTime(model.Pharmacy.LicenseExpiresDate);
                    pharmacy.NpiNumber = model.Pharmacy.NpiNumber;
                    pharmacy.ReferCode = model.Pharmacy.ReferCode;
                    pharmacy.PharmacyName = model.Pharmacy.PharmacyName;
                    pharmacy.PharmacyAdminUser.FirstName = model.Pharmacy.FirstName;
                    pharmacy.PharmacyAdminUser.LastName = model.Pharmacy.LastName;
                    pharmacy.PharmacyAdminUser.JobTitle = model.Pharmacy.JobTitle;
                    pharmacy.PharmacyAdminUser.Email = model.Pharmacy.PrimaryEmail;
                    pharmacy.PharmacyAdminUser.PhoneNumber = model.Pharmacy.PhoneNumber;
                    pharmacy.PharmacyAdminUser.Mobile = model.Pharmacy.MobileNumber;

                    await _pharmacy.UpdateAsync(pharmacy, Accessor, User.GetUserId());

                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.UserUpdatedSuccessfully);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/SavePharmacy");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Common
        private void BindDropdownList()
        {
            BindStateList();
            ViewBag.PharmacySystemMaster = _pharmacySystem.GetAll().Select(x => new SelectListItem { Text = x.PharmacySystemName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.PharmacyTypeMaster = _pharmacyTypeMaster.GetAll().Select(x => new SelectListItem { Text = x.PharmacyTypeName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        private void BindStateList()
        {
            ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        private void ResetDefaultAddress(bool isBilling, long pharmacyId)
        {
            if (isBilling)
            {
                var allBill = _pharmacyBilling.GetAll(x => x.PharmacyId == pharmacyId && x.IsDefault).ToList();
                allBill.ForEach(x => { x.IsDefault = false; });
                foreach (var item in allBill)
                {
                    _pharmacyBilling.Update(item);
                }
            }
            else
            {
                var allBill = _pharmacyShipping.GetAll(x => x.PharmacyId == pharmacyId && x.IsDefault).ToList();
                allBill.ForEach(x => { x.IsDefault = false; });
                foreach (var item in allBill)
                {
                    _pharmacyShipping.Update(item);
                }
            }
        }
        #endregion
    }
}