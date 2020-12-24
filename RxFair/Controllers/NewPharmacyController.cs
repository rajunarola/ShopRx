using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Controllers
{
    public class NewPharmacyController : BaseController<NewPharmacyController>
    {
        #region Fields
        private readonly IStateService _state;
        private readonly IPharmacyService _pharmacy;
        private readonly IPharmacySystemMasterService _pharmacySystem;
        private readonly IPharmacyTypeMasterService _pharmacyType;
        private readonly IPharmacyBillingAddressService _pharmacyBilling;
        private readonly IPharmacyShippingAddressService _pharmacyShipping;
        private readonly IRewardEarnService _rewardEarn;
        private readonly IRewardMoneyMasterService _rewardMoney;
        private readonly EmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Ctor
        public NewPharmacyController(IHostingEnvironment environment, IErrorLogService errorLog, IMapper mapper, IHttpContextAccessor accessor, IStateService state, IPharmacyService pharmacy, IPharmacySystemMasterService pharmacySystem, IPharmacyTypeMasterService pharmacyType,
            IOptions<EmailSettingsGmail> emailSettingsGmail, IPharmacyBillingAddressService pharmacyBilling, IPharmacyShippingAddressService pharmacyShipping, IRewardEarnService rewardEarn, IRewardMoneyMasterService rewardMoney, UserManager<ApplicationUser> userManager)
        {
            _state = state;
            _pharmacy = pharmacy;
            _pharmacySystem = pharmacySystem;
            _pharmacyType = pharmacyType;
            _emailService = new EmailService(emailSettingsGmail);
            _pharmacyBilling = pharmacyBilling;
            _pharmacyShipping = pharmacyShipping;
            _rewardEarn = rewardEarn;
            _rewardMoney = rewardMoney;
            _userManager = userManager;
        }
        #endregion

        #region Methods
        public async Task<IActionResult> Index(long id = 0)
        {
            BindDropdownList();
            if (id == 0) return View(new NewPharmacyDto { Id = id });
            var user = await _userManager.FindByIdAsync(id.ToString());
            var model = new NewPharmacyDto
            {
                Id = user.Id,
                PrimaryEmail = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle,
                PhoneNumber = user.PhoneNumber,
                MobileNumber = user.Mobile
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterNewPharmacy(NewPharmacyDto model, bool isBillingAddress = false)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    #region Pharmacy User

                    ApplicationUser newPharmacy;
                    if (model.Id == 0)
                    {
                        var isExist = await _userManager.FindByEmailAsync(model.Email);
                        if (isExist != null)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, GlobalConstant.AlreadyRegisterd);
                        }
                        newPharmacy = new ApplicationUser
                        {
                            Email = model.Email,
                            UserName = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            JobTitle = model.JobTitle,
                            PhoneNumber = model.PhoneNumber,
                            Mobile = model.MobileNumber??"",
                            IsActive = true
                        };
                        var result = await _userManager.CreateAsync(newPharmacy, model.Password);
                        if (!result.Succeeded)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, GlobalConstant.NotRegistered);
                        }
                        await _userManager.AddToRoleAsync(newPharmacy, UserRoles.PharmacyPrimaryAdmin);
                    }
                    else
                    {
                        newPharmacy = await _userManager.FindByIdAsync(model.Id.ToString());
                        newPharmacy.FirstName = model.FirstName;
                        newPharmacy.LastName = model.LastName;
                        newPharmacy.JobTitle = model.JobTitle;
                        newPharmacy.PhoneNumber = model.PhoneNumber;
                        newPharmacy.Mobile = model.MobileNumber;
                    }
                    #endregion

                    #region DeaFile & LicenseFile
                    string newDeaFile = string.Empty, newLicenseFile = string.Empty;
                    if (model.DeaFile != null && model.LicenseFile != null)
                    {
                        newDeaFile = $@"Dea-{CommonMethod.GetFileName(model.DeaFile.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Pharmacy, newDeaFile, model.DeaFile);

                        newLicenseFile = $@"License-{CommonMethod.GetFileName(model.LicenseFile.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Pharmacy, newLicenseFile, model.LicenseFile);
                    }
                    #endregion

                    #region Create Pharmacy
                    var culture = new System.Globalization.CultureInfo("en-GB");
                    model.DeaExpries = Convert.ToDateTime(model.DeaExpriesDate, culture);
                    model.LicenseExpires = Convert.ToDateTime(model.LicenseExpiresDate, culture);
                    var objPharmacy = new Pharmacy();

                    objPharmacy.DeaNumber = model.DeaNumber;
                    objPharmacy.DeaExpires = model.DeaExpries;
                    objPharmacy.DeaFile = newDeaFile;
                    objPharmacy.LicenseNumber = model.LicenseNumber;
                    objPharmacy.LicenseExpires = model.LicenseExpires;
                    objPharmacy.LicenseFile = newLicenseFile;
                    objPharmacy.NpiNumber = model.NpiNumber;
                    objPharmacy.PharmacyName = model.PharmacyName;
                    objPharmacy.PharmacySystemId = model.PharmacySystemId;
                    objPharmacy.PharmacyTypeId = model.PharmacyTypeId;
                    objPharmacy.Status = (int)GlobalEnums.PharmacyStatus.Pending;
                    objPharmacy.UserId = newPharmacy.Id;
                    objPharmacy.IsActive = true;
                    

                    try
                    {
                        var isExist = _pharmacy.GetSingle(x => x.UserId == newPharmacy.Id);
                        if (isExist != null)
                        {
                            var Updated_Result = await _pharmacy.UpdateAsync(objPharmacy, Accessor);
                        }
                        else{
                            var Inserted_Result = await _pharmacy.InsertAsync(objPharmacy, Accessor);
                        }
                       
                    }
                    catch (Exception e)
                    {

                        throw;
                    }
                
                    newPharmacy.PharmacyId = objPharmacy.Id;
                    await _userManager.UpdateAsync(newPharmacy);
                    #endregion

                    #region Reward Referral Points
                    if (!string.IsNullOrEmpty(model.ReferCode))
                    {
                        var referalPharmacy = _pharmacy.GetSingle(x => x.ReferCode == model.ReferCode);
                        if (referalPharmacy != null)
                        {
                            var rewardMoney = _rewardMoney.GetSingle(x => x.RewardTypeId == (long)GlobalEnums.RewardType.Referral && x.IsActive);
                            if (rewardMoney != null && rewardMoney.Id != 0)
                            {
                                var rewardEarn = Mapper.Map<RewardEarnDto, RewardEarn>(CommonMethod.NewRewardEarnDto((long)GlobalEnums.RewardType.Referral, referalPharmacy.Id));
                                //rewardEarn.PharmacyId = rewardMoney.Id;
                                rewardEarn.RewardMoney = rewardMoney.Referral;
                                rewardEarn.IsActive = true;
                                await _rewardEarn.InsertAsync(rewardEarn, Accessor);
                            }
                        }
                    }
                    #endregion

                    #region Billing & Delivery Address
                    var billingAddress = new PharmacyBillingAddress
                    {
                        Address1 = isBillingAddress ? model.BillAddress1 : model.DeliveryAddress1,
                        Address2 = isBillingAddress ? model.BillAddress2 : model.DeliveryAddress2,
                        City = isBillingAddress ? model.BillCity : model.DeliveryCity,
                        PharmacyId = objPharmacy.Id,
                        ZipCode = isBillingAddress ? model.BillZipCode : model.DeliveryZipCode,
                        StateId = isBillingAddress ? model.BillState : model.DeliveryState,
                        IsDefault = true
                    };
                    await _pharmacyBilling.InsertAsync(billingAddress, Accessor);

                    var shipingAddress = new PharmacyShippingAddress
                    {
                        Address1 = isBillingAddress ? model.BillAddress1 : model.DeliveryAddress1,
                        Address2 = isBillingAddress ? model.BillAddress2 : model.DeliveryAddress2,
                        City = isBillingAddress ? model.BillCity : model.DeliveryCity,
                        PharmacyId = objPharmacy.Id,
                        ZipCode = isBillingAddress ? model.BillZipCode : model.DeliveryZipCode,
                        StateId = isBillingAddress ? model.BillState : model.DeliveryState,
                        IsDefault = true
                    };
                    await _pharmacyShipping.InsertAsync(shipingAddress, Accessor);
                    #endregion

                    txscope.Complete();

                    #region Send Email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newPharmacy);
                    var callbackUrl = Url.EmailConfirmationLink(newPharmacy.Id, code, Request.Scheme);
                    string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.NewPharmacy, GetPhysicalUrl());
                    emailTemplate = emailTemplate.Replace("{PharmacyName}", objPharmacy.PharmacyName);
                    emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);
                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                    {
                        BodyText = emailTemplate,
                        Subject = "New Pharmacy Created",
                        ToAddress = newPharmacy.Email
                    });
                    #endregion

                    #region Congratulation email
                    //var congratulationTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.Congratulation, GetPhysicalUrl());
                    //congratulationTemplate = congratulationTemplate.Replace("{acounttype}", "Pharmacy");
                    //congratulationTemplate = congratulationTemplate.Replace("{email}", newPharmacy.Email);
                    //await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                    //{
                    //    ToAddress = newPharmacy.Email,
                    //    Subject = "Congratulation",
                    //    BodyText = congratulationTemplate
                    //});
                    #endregion

                    return JsonResponse.GenerateJsonResult(1, "New pharmacy created successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-RegisterNewPharmacy");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult CheckReferCode(string refercode)
        {
            var isExist = _pharmacy.GetSingle(x => x.ReferCode == refercode);
            return JsonResponse.GenerateJsonResult(isExist != null ? 1 : 0, "Invalid refer code.");
        }
        #endregion

        #region Controller Common
        private void BindDropdownList()
        {
            ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.PharmacySystemMaster = _pharmacySystem.GetAll().Select(x => new SelectListItem() { Text = x.PharmacySystemName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.PharmacyTypeMaster = _pharmacyType.GetAll().Select(x => new SelectListItem() { Text = x.PharmacyTypeName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}