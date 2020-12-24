using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;

namespace RxFair.Controllers
{
    public class NewDistributorController : BaseController<NewDistributorController>
    {
        #region Fields
        private readonly IDistributorService _distributor;
        private readonly IDistributerOrderSettingService _distributorSetting;
        private readonly EmailService _emailService;
        private readonly INewDistributorRequestService _newDistributor;
        private readonly IStateService _state;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAddressService _userAddress;
        #endregion

        #region Ctor
        public NewDistributorController(IDistributorService distributor, IOptions<EmailSettingsGmail> emailSettingsGmail, INewDistributorRequestService newDistributor, IStateService state, IUserAddressService userAddress, UserManager<ApplicationUser> userManager, IDistributerOrderSettingService distributorSetting)
        {
            _distributor = distributor;
            _emailService = new EmailService(emailSettingsGmail);
            _newDistributor = newDistributor;
            _state = state;
            _userManager = userManager;
            _distributorSetting = distributorSetting;
            _userAddress = userAddress;
        }
        #endregion

        #region Methods
        public async Task<IActionResult> Index(long id = 0)
        {
            BindDropdownList();
            if (id == 0) return View(new NewDistributorRequestDto { Id = id });
            var user = await _userManager.FindByIdAsync(id.ToString());
            var model = new NewDistributorRequestDto()
            {
                Id = user.Id,
                Email = user.Email.Trim(),
                Mobile = user.Mobile,
                Phone = user.PhoneNumber,
                IsActive = true
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NewDistributorRequest(NewDistributorRequestDto model)
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
                    model.Email = model.Email.Trim();

                    if (model.Id == 0)
                    {
                        if (_newDistributor.GetCount(x => !x.IsActive && x.Email.Trim().ToLower().Equals(model.Email.ToLower())) != 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, @"Your previous request has been rejected by admin. You can't send another request.");
                        }
                        var objDistributor = new NewDistributorRequest
                        {
                            CompanyName = model.CompanyName,
                            Mobile = model.Mobile,
                            Phone = model.Phone,
                            ZipCode = model.ZipCode,
                            Address = model.Address,
                            StateId = model.StateId,
                            City = model.City,
                            ContactName = model.ContactName,
                            ContactEmail = model.ContactEmail,
                            ContactMobile = model.ContactMobile,
                            ContactAddress = model.ContactAddress,
                            ContactCity = model.ContactCity,
                            ContactStateId = model.ContactStateId,
                            ContactZipCode = model.ContactZipCode,
                            Email = model.Email,
                            IsActive = true
                        };
                        await _newDistributor.InsertAsync(objDistributor, Accessor);
                    }
                    else
                    {
                        var distributorUser = await _userManager.FindByIdAsync(model.Id.ToString());
                        distributorUser.Mobile = model.Mobile;
                        distributorUser.PhoneNumber = model.Phone;

                        #region Create Distributor
                        var newdistributor = new Distributor
                        {
                            UserId = distributorUser.Id,
                            CompanyName = model.CompanyName,
                            Email = distributorUser.Email,
                            Mobile = model.Mobile,
                            Phone = model.Phone,
                            Address = model.Address,
                            ZipCode = model.ZipCode,
                            StateId = model.StateId,
                            City = model.City,
                            ContactName = model.ContactName,
                            ContactEmail = model.ContactEmail,
                            ContactMobile = model.ContactMobile,
                            ContactAddress = model.ContactAddress,
                            ContactCity = model.ContactCity,
                            ContactStateId = model.ContactStateId,
                            ContactZipCode = model.ContactZipCode
                        };
                        await _distributor.InsertAsync(newdistributor, Accessor);


                        await _distributorSetting.InsertAsync(new DistributorOrderSetting
                        {
                            DistributorId = newdistributor.Id,
                            TimeZoneId = 22,
                            MinOrderAmount = 0,
                            ShippingCharge = 0,
                            OverNightAmount = 0,
                            ServiceDayMonday = false,
                            ServiceDayTuesday = false,
                            ServiceDayWednesday = false,
                            ServiceDayThursday = false,
                            ServiceDayFriday = false,
                            ServiceDaySaturday = false,
                            ServiceDaySunday = false
                        }, Accessor);

                        var address = new UserAddress
                        {
                            City = model.City,
                            StateId = model.StateId,
                            ZipCode = model.ZipCode,
                            CreatedBy = distributorUser.Id,
                            IsActive = true
                        };
                        await _userAddress.InsertAsync(address, Accessor, distributorUser.Id);
                        #endregion

                        distributorUser.DistributorId = newdistributor.Id;
                        await _userManager.UpdateAsync(distributorUser);
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Distributor {(model.Id == 0 ? "request sent" : "created")} successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/NewDistributorRequest");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Controller Common
        private void BindDropdownList()
        {
            ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}