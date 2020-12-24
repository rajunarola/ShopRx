using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class DistributorProfileController : BaseController<MyAccountController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IUserService _user;
        private readonly IUserAddressService _useraddress;
        private readonly IStateService _state;
        private readonly ITimeZoneService _timeZone;
        private readonly IDistributerOrderSettingService _distributerOrder;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region ctor
        public DistributorProfileController(IDistributorService distributor, IUserService user, IUserAddressService userAddress, IStateService state, IDistributerOrderSettingService distributerOrderSetting, ITimeZoneService timeZone, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _distributor = distributor;
            _user = user;
            _useraddress = userAddress;
            _state = state;
            _distributerOrder = distributerOrderSetting;
            _timeZone = timeZone;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Method

        public IActionResult Index()
        {
            try
            {
                BindDropDownList();
                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var distributor = _distributor.GetSingle(x => x.DistributorAdminUser.DistributorId == DistributorId);
                var dtoAdmin = Mapper.Map<DistributorProfileDto>(distributor);
                dtoAdmin.FirstName = distributor.DistributorAdminUser.FirstName;
                dtoAdmin.LastName = distributor.DistributorAdminUser.LastName;
                dtoAdmin.UserProfileImage = distributor.CompanyLogo;

                int i = 0;
                i = (dtoAdmin.Address.Equals(dtoAdmin.ContactAddress)) ? i + 1 : i + 0;
                i = (dtoAdmin.City.Equals(dtoAdmin.ContactCity)) ? i + 1 : i + 0;
                i = (dtoAdmin.StateName.Equals(dtoAdmin.ContactStateName)) ? i + 1 : i + 0;
                i = (dtoAdmin.ZipCode.Equals(dtoAdmin.ContactZipCode)) ? i + 1 : i + 0;
                dtoAdmin.SameAsCompany = i == 4;

                var orderSetting = distributor.DistributorOrderSettings;
                if (orderSetting == null)
                {
                    dtoAdmin.DistributerOrderSettingDto = new DistributerOrderSettingDto { Id = 0, DistributorId = distributor.Id, TimeZoneId = 0 };
                    return View(dtoAdmin);
                }
                dtoAdmin.DistributerOrderSettingDto = Mapper.Map<DistributerOrderSettingDto>(orderSetting);
                dtoAdmin.DistributerOrderSettingDto.DistributorId = distributor.Id;
                return View(dtoAdmin);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "DistributorProfile-Index");
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditDistributor(DistributorProfileDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var distributor = await _distributor.GetSingleAsync(x => x.Id == model.Id);
  
                        distributor.CompanyName = model.CompanyName;
                        distributor.Email = model.Email;
                        distributor.Mobile = model.Mobile;
                        distributor.Address = model.Address;
                        distributor.City = model.City;
                        distributor.StateId = model.StateId;
                        distributor.ZipCode = model.ZipCode;
                        distributor.Phone = model.Phone;
                        distributor.ContactName = model.ContactName;
                        distributor.ContactEmail = model.ContactEmail;
                        distributor.ContactMobile = model.ContactMobile;
                        distributor.ContactAddress = model.ContactAddress;
                        distributor.ContactCity = model.ContactCity;
                        distributor.ContactStateId = model.ContactStateId;
                        distributor.ContactZipCode = model.ContactZipCode;

                        await _distributor.UpdateAsync(distributor, Accessor, User.GetUserId());

                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, GlobalConstant.UserUpdatedSuccessfully);
                    }
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EditDistributor");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrderSetting(DistributorProfileDto profileModel)
        {
            var model = profileModel.DistributerOrderSettingDto;
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
                                    ? DateTime.Parse(model.MondayCutOffTime).TimeOfDay
                                    : (TimeSpan?) null,
                            TuesdayCutOffTime = model.ServiceDayTuesday
                                ? DateTime.Parse(model.TuesdayCutOffTime).TimeOfDay
                                : (TimeSpan?) null,
                            WednesdayCutOffTime = model.ServiceDayWednesday
                                ? DateTime.Parse(model.WednesdayCutOffTime).TimeOfDay
                                : (TimeSpan?) null,
                            ThursdayCutOffTime = model.ServiceDayThursday
                                ? DateTime.Parse(model.ThursdayCutOffTime).TimeOfDay
                                : (TimeSpan?) null,
                            FridayCutOffTime =
                                model.ServiceDayFriday
                                    ? DateTime.Parse(model.FridayCutOffTime).TimeOfDay
                                    : (TimeSpan?) null,
                            SaturdayCutOffTime = model.ServiceDaySaturday
                                ? DateTime.Parse(model.SaturdayCutOffTime).TimeOfDay
                                : (TimeSpan?) null,
                            SundayCutOffTime =
                                model.ServiceDaySunday
                                    ? DateTime.Parse(model.SundayCutOffTime).TimeOfDay
                                    : (TimeSpan?) null,
                        };

                        await _distributerOrder.InsertAsync(orderSetting, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Distributor Order Setting created successfully.");
                    }

                    editOrder.DistributorId = model.DistributorId;
                    editOrder.TimeZoneId = model.TimeZoneId;

                    editOrder.ShippingCharge = model.ShippingCharge;
                    editOrder.OverNightAmount = model.OverNightAmount;
                    editOrder.MinOrderAmount = model.MinOrderAmount;

                    editOrder.ServiceDayMonday = model.ServiceDayMonday;
                    editOrder.ServiceDayTuesday = model.ServiceDayTuesday;
                    editOrder.ServiceDayWednesday = model.ServiceDayWednesday;
                    editOrder.ServiceDayThursday = model.ServiceDayThursday;
                    editOrder.ServiceDayFriday = model.ServiceDayFriday;
                    editOrder.ServiceDaySaturday = model.ServiceDaySaturday;
                    editOrder.ServiceDaySunday = model.ServiceDaySunday;

                    editOrder.MondayCutOffTime = model.ServiceDayMonday ? DateTime.Parse(model.MondayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.TuesdayCutOffTime = model.ServiceDayTuesday ? DateTime.Parse(model.TuesdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.WednesdayCutOffTime = model.ServiceDayWednesday ? DateTime.Parse(model.WednesdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.ThursdayCutOffTime = model.ServiceDayThursday ? DateTime.Parse(model.ThursdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.FridayCutOffTime = model.ServiceDayFriday ? DateTime.Parse(model.FridayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.SaturdayCutOffTime = model.ServiceDaySaturday ? DateTime.Parse(model.SaturdayCutOffTime).TimeOfDay : (TimeSpan?)null;
                    editOrder.SundayCutOffTime = model.ServiceDaySunday ? DateTime.Parse(model.SundayCutOffTime).TimeOfDay : (TimeSpan?)null;

                    await _distributerOrder.UpdateAsync(editOrder, Accessor, User.GetUserId());
                    txscope.Complete();

                    return JsonResponse.GenerateJsonResult(1, "Distributor Order Setting updated successfully.", editOrder.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EditOrderSetting");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public async Task<IActionResult> UploadProfile(IFormFile profileImage)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string newProfileFile = string.Empty;
                try
                {
                    if (profileImage == null)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(1, "Profile changed successfully.");
                    }

                    var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                    newProfileFile = CommonMethod.GetFileName(profileImage.FileName);
                    await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.UserProfile, newProfileFile, profileImage);
                    var distributor = _distributor.GetById(distributorId);
                    var oldProfile = distributor.CompanyLogo;
                    distributor.CompanyLogo = newProfileFile;
                    await _distributor.UpdateAsync(distributor, Accessor);

                    txscope.Complete();

                    if (!string.IsNullOrEmpty(oldProfile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.UserProfile, oldProfile), true);
                    }
                    return JsonResponse.GenerateJsonResult(1, "Profile changed successfully.", $@"\{FilePathList.UserProfile}\{newProfileFile}");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    if (!string.IsNullOrEmpty(newProfileFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.UserProfile, newProfileFile), true);
                    }
                    ErrorLog.AddErrorLog(ex, "UploadProfile");
                    return JsonResponse.GenerateJsonResult(1, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region Controller Common

        public void BindDropDownList()
        {
            ViewBag.TimeZoneList = _timeZone.GetAll().Select(x => new SelectListItem { Text = x.StandardName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.State = _state.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }

        #endregion
    }
}