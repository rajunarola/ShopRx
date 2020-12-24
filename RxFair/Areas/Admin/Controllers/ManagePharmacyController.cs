using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
    public class ManagePharmacyController : BaseController<ManagePharmacyController>
    {
        #region Fields
        private readonly EmailService _emailService;
        private readonly IPharmacyService _pharmacy;
        private readonly IPharmacyBillingAddressService _pharmacyBill;
        private readonly IPharmacyShippingAddressService _pharmacyShip;
        private readonly IPharmacySystemMasterService _pharmacySystem;
        private readonly IPharmacyTypeMasterService _pharmacyType;
        private readonly IUserService _user;
        private readonly IStateService _state;
        #endregion

        #region Ctor
        public ManagePharmacyController(IOptions<EmailSettingsGmail> emailSettingsGmail, IPharmacyService pharmacy, IPharmacyBillingAddressService pharmacyBillingAddress, IPharmacyShippingAddressService pharmacyShippingAddress, IPharmacySystemMasterService pharmacySystemMaster, IPharmacyTypeMasterService pharmacyType, IUserService user, IStateService state)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _pharmacy = pharmacy;
            _pharmacyBill = pharmacyBillingAddress;
            _pharmacyShip = pharmacyShippingAddress;
            _pharmacySystem = pharmacySystemMaster;
            _pharmacyType = pharmacyType;
            _user = user;
            _state = state;
        }
        #endregion

        #region Methods

        [HttpGet]
        public IActionResult Index(string id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManagePharmacyStatus(long id, int status)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacy = _pharmacy.GetSingle(x => x.Id == id);
                    pharmacy.Status = status;
                    if (status == (int)GlobalEnums.PharmacyStatus.Accepted)
                    {
                        var referCode = CommonMethod.CreateRandomPassword(8);
                        var code = referCode;
                        var isExist = _pharmacy.GetSingle(x => x.ReferCode.Equals(code));
                        if (isExist != null)
                        {
                            referCode = CommonMethod.CreateRandomPassword(8);
                        }
                        pharmacy.ReferCode = referCode;
                    }
                    await _pharmacy.UpdateAsync(pharmacy, Accessor, User.GetUserId());

                    if (status != (int)GlobalEnums.PharmacyStatus.Pending)
                    {
                        string physicalUrl = GetPhysicalUrl();
                        string subject = status == 2 ? "accepted" : "rejected";
                        string emailTemplate = string.Empty;
                        if (status == 2)
                        {
                            emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.PharmacyCongratulation, physicalUrl);
                            emailTemplate = emailTemplate.Replace("{PharmacyName}", pharmacy.PharmacyName);
                            subject = $@"Congrats your pharmacy activated";
                        }
                        else
                        {
                            emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.PharmacyStatus, physicalUrl);
                            emailTemplate = emailTemplate.Replace("{UserName}", pharmacy.PharmacyAdminUser.FullName);
                            emailTemplate = emailTemplate.Replace("{status}", subject);
                            subject = $@"Pharmacy {subject}";
                        }
                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                        {
                            ToAddress = pharmacy.PharmacyAdminUser.Email,
                            Subject = subject,
                            BodyText = emailTemplate
                        });

                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Pharmacy status changed successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-ManagePharmacyStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult ViewPharmacyAccount(long id)
        {
            if (id == 0) return View(@"Components/_ViewPharmacyAccount", new NewPharmacyDto { Id = id });

            var pharmacy = _pharmacy.GetSingle(x => x.Id == id);
            var billingAddresse = pharmacy.BillingAddresses.FirstOrDefault();
            var shippingAddress = pharmacy.ShippingAddresses.FirstOrDefault();

            var pharmacyView = new NewPharmacyDto
            {
                Id = pharmacy.Id,
                BillAddress1 = billingAddresse?.Address1 ?? "",
                BillAddress2 = billingAddresse?.Address2 ?? "",
                BillCity = billingAddresse?.City ?? "",
                BillStatName = billingAddresse?.State.Name,
                BillZipCode = billingAddresse?.ZipCode ?? "",

                DeliveryAddress1 = shippingAddress?.Address1 ?? "",
                DeliveryAddress2 = shippingAddress?.Address2 ?? "",
                DeliveryCity = shippingAddress?.City ?? "",
                DeliveryStatName = shippingAddress?.State.Name,
                DeliveryZipCode = shippingAddress?.ZipCode ?? "",

                FirstName = pharmacy.PharmacyAdminUser.FirstName,
                LastName = pharmacy.PharmacyAdminUser.LastName,
                JobTitle = pharmacy.PharmacyAdminUser.JobTitle,
                PrimaryEmail = pharmacy.PharmacyAdminUser.Email,
                PhoneNumber = pharmacy.PharmacyAdminUser.PhoneNumber,
                LicenseNumber = pharmacy.LicenseNumber,
                LicenseExpires = pharmacy.LicenseExpires,
                DeaNumber = pharmacy.DeaNumber,
                DeaExpries = pharmacy.DeaExpires,
                NpiNumber = pharmacy.NpiNumber,
                PharmacyName = pharmacy.PharmacyName,
                PharmacySystemName = pharmacy.PharmacySystemMaster.PharmacySystemName,
                PharmacyTypeName = pharmacy.PharmacyTypeMaster.PharmacyTypeName

            };

            return View(@"Components/_ViewPharmacyAccount", pharmacyView);
        }

        [HttpGet]
        public IActionResult RemovePharmacy(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var pharmacy = _pharmacy.GetSingle(x => x.Id == id);
                    _pharmacy.Delete(pharmacy);
                    _pharmacy.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Pharmacy deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Get-RemovePharmacy");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public async Task<IActionResult> ViewPharmacy(long id)
        {
            try
            {
                var pharmacy = await _pharmacy.GetSingleAsync(x => x.Id == id);
                if (pharmacy == null) return JsonResponse.GenerateJsonResult(0, GlobalConstant.UserNotFound);
                return JsonResponse.GenerateJsonResult(1, "", pharmacy);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get-ViewPharmacy");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPharmacyRequestList(JQueryDataTableParamModel param, int status)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@Status", SqlDbType.NVarChar) { Value = status });

                var allslotList = await _pharmacy.GetNewPharmacyRequestList(parameters.ToArray());
                var total = allslotList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allslotList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetPharmacyRequestList");
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
        public async Task<IActionResult> ManageIsActive(long id)
        {
            try
            {
                var pharmacy = _pharmacy.GetSingle(x => x.Id == id);
                pharmacy.IsActive = !pharmacy.IsActive;
                await _pharmacy.UpdateAsync(pharmacy, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Pharmacy {(pharmacy.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-ManageIsActive");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }

        }
        #endregion

        #region Controller Common
        #endregion

    }
}

