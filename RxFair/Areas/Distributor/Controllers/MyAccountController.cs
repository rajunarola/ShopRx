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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http.Extensions;
using RxFair.Utility.Extension;
using PayPal.Api;
using RxFair.Controllers;
using RxFair.Utility.Helpers;
using RxFair.Service.Utility;
using Microsoft.Extensions.Options;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class MyAccountController : BaseController<MyAccountController>
    {
        #region Fields
        private readonly IUserService _user;
        private readonly IUserAddressService _userAddress;
        private readonly IStateService _state;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDistributorService _distributor;
        private readonly IDistributorSubscriptionService _distributorSubscription;
        private readonly IDistributorSubscriptionHistoryService _distributorSubscriptionHistory;
        private readonly ISubscriptionTypeService _subscriptionType;
        private readonly IDistributorDocumentService _distributorDocument;
        private readonly IPayPalService _payPal;
        private readonly EmailService _emailService;
        private ISession Session => Accessor.HttpContext.Session;
        private Payment _payment;

        #endregion

        #region ctor
        public MyAccountController(IDistributorService distributor, IDistributorDocumentService distributorDocument, IUserService user,
            IUserAddressService userAddress, IStateService state, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IOptions<EmailSettingsGmail> emailSettingsGmail, IDistributorSubscriptionService distributorSubscription, IDistributorSubscriptionHistoryService distributorSubscriptionHistory,
            ISubscriptionTypeService subscriptionType,
            IPayPalService payPal)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _user = user;
            _userAddress = userAddress;
            _state = state;
            _userManager = userManager;
            _signInManager = signInManager;
            _distributorSubscription = distributorSubscription;
            _distributorSubscriptionHistory = distributorSubscriptionHistory;
            _subscriptionType = subscriptionType;
            _distributor = distributor;
            _distributorDocument = distributorDocument;
            _payPal = payPal;
        }
        #endregion

        #region Method

        public IActionResult Index()
        {
            BindDropdownList();
            var result = _user.GetSingle(x => x.Id == User.GetUserId());
            var address = result.UserAddress?.FirstOrDefault();
            var dtoModel = new UserProfileDto
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                Mobile = result.Mobile,
                JobTitle = result.JobTitle,
                City = address?.City ?? "",
                StateId = address?.StateId ?? 0,
                ZipCode = address?.ZipCode ?? "",
                UserProfileImage = result.UserProfileImage
            };
            return View(dtoModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(UserProfileDto model)
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
                    var result = await _userManager.FindByIdAsync(model.Id.ToString());
                    result.FirstName = model.FirstName;
                    result.LastName = model.LastName;
                    result.Mobile = model.Mobile;
                    result.JobTitle = model.JobTitle;
                    await _user.UpdateAsync(result, Accessor, model.Id);
                    await _signInManager.SignInAsync(result, isPersistent: false);

                    var address = result.UserAddress?.FirstOrDefault();
                    if (address != null)
                    {
                        address.City = model.City;
                        address.StateId = model.StateId;
                        address.ZipCode = model.ZipCode;
                        address.CreatedBy = result.Id;
                        await _userAddress.UpdateAsync(address, Accessor, result.Id);
                    }
                    else
                    {
                        address = new UserAddress
                        {
                            City = model.City,
                            StateId = model.StateId,
                            ZipCode = model.ZipCode,
                            CreatedBy = result.Id,
                            IsActive = true
                        };
                        await _userAddress.InsertAsync(address, Accessor, result.Id);
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Your profile information updated successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/MyProfile");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public async Task<IActionResult> ChangePassword(UserProfileDto model)
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
                    var currUser = await _userManager.FindByIdAsync(User.GetUserId().ToString());

                    var result = await _userManager.ChangePasswordAsync(currUser, model.OldPassword, model.NewPassword);
                    if (!result.Succeeded)
                    {
                        //AddErrors(result);
                        var a = result.Errors.FirstOrDefault();
                        if (a?.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError(string.Empty, "Old password is incorrect.");
                        }
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }
                    await _signInManager.SignInAsync(currUser, isPersistent: false);

                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Password changed successfully.");

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ChangePassword");
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
                    newProfileFile = CommonMethod.GetFileName(profileImage.FileName);
                    await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.UserProfile, newProfileFile, profileImage);
                    var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
                    var oldProfile = user.UserProfileImage;
                    user.UserProfileImage = newProfileFile;
                    await _user.UpdateAsync(user, Accessor);

                    await _signInManager.RefreshSignInAsync(user);
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

        public IActionResult ManageDocuments()
        {
            try
            {
                var DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                var result = _distributor.GetSingle(x => x.DistributorAdminUser.DistributorId == DistributorId);
                if (result == null) return View(new DistributorDocumentMasterView());
                return View(new DistributorDocumentMasterView
                {
                    Id = result.DistributorDocumentMaster?.Id ?? 0,
                    DistributorId = result.Id,
                    LicenseCertificateFile = result.DistributorDocumentMaster?.LicenseCertificate ?? "",
                    ReturnPolicyFile = result.DistributorDocumentMaster?.ReturnPolicy ?? "",
                    WaiverFile = result.DistributorDocumentMaster?.Waiver ?? ""
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Get/ManageDocuments");
                return View(new DistributorDocumentMasterView());
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageDocuments(DistributorDocumentMasterView model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string licenseFile = string.Empty, returnFile = string.Empty, waiverFile = string.Empty;
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }
                    #region LicenseFile & ReturnFile & WaiverFile
                    if (model.LicenseCertificate != null)
                    {
                        licenseFile = $@"License-{CommonMethod.GetFileName(model.LicenseCertificate.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Document, licenseFile, model.LicenseCertificate);
                    }
                    if (model.ReturnPolicy != null)
                    {
                        returnFile = $@"ReturnPolicy-{CommonMethod.GetFileName(model.ReturnPolicy.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Document, returnFile, model.ReturnPolicy);
                    }
                    if (model.Waiver != null)
                    {
                        waiverFile = $@"Waiver-{CommonMethod.GetFileName(model.Waiver.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Document, waiverFile, model.Waiver);
                    }
                    #endregion
                    if (model.Id == 0)
                    {
                        var document = new DistributorDocumentMaster
                        {
                            LicenseCertificate = licenseFile,
                            ReturnPolicy = returnFile,
                            Waiver = waiverFile,
                            IsActive = true,
                            DistributorId = model.DistributorId
                        };
                        await _distributorDocument.InsertAsync(document, Accessor, User.GetUserId());

                        txscope.Complete();
                        return RedirectToAction("ManageDocuments", "MyAccount", new { area = "Distributor" });
                    }
                    var result = _distributorDocument.GetSingle(x => x.Id == model.Id);
                    var oldLicenseFile = result.LicenseCertificate;
                    var oldReturnPolicy = result.ReturnPolicy;
                    var oldWaiverFile = result.Waiver;

                    result.LicenseCertificate = !string.IsNullOrEmpty(licenseFile) ? licenseFile : oldLicenseFile;
                    result.ReturnPolicy = !string.IsNullOrEmpty(returnFile) ? returnFile : oldReturnPolicy;
                    result.Waiver = !string.IsNullOrEmpty(waiverFile) ? waiverFile : oldWaiverFile;
                    result.DistributorId = model.DistributorId;
                    await _distributorDocument.UpdateAsync(result, Accessor, result.Id);

                    txscope.Complete();

                    if (!string.IsNullOrEmpty(licenseFile) && !string.IsNullOrEmpty(oldLicenseFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, oldLicenseFile), true);
                    }
                    if (!string.IsNullOrEmpty(returnFile) && !string.IsNullOrEmpty(oldReturnPolicy))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, oldReturnPolicy), true);
                    }
                    if (!string.IsNullOrEmpty(waiverFile) && !string.IsNullOrEmpty(oldWaiverFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, oldWaiverFile), true);
                    }
                    TempData["Message"] = "Document uploaded successfully.";
                    TempData.Keep();
                    return RedirectToAction("ManageDocuments", "MyAccount", new { area = "Distributor" });
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    if (!string.IsNullOrEmpty(licenseFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, licenseFile), true);
                    }
                    ErrorLog.AddErrorLog(ex, "Post/ManageDocuments");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.FileMissing);
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteDocumentFile(long id, string file)
        {
            try
            {
                if (!ModelState.IsValid) return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                var distributerObj = _distributorDocument.GetById(id);

                if (file.Equals(distributerObj.LicenseCertificate))
                {
                    CommonMethod.DeleteFile(
                        CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document,
                            distributerObj.LicenseCertificate), true);
                    distributerObj.LicenseCertificate = string.Empty;
                }

                if (file.Equals(distributerObj.ReturnPolicy))
                {
                    CommonMethod.DeleteFile(
                        CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document,
                            distributerObj.ReturnPolicy), true);
                    distributerObj.ReturnPolicy = string.Empty;
                }

                if (file.Equals(distributerObj.Waiver))
                {
                    CommonMethod.DeleteFile(
                        CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document,
                            distributerObj.Waiver), true);
                    distributerObj.Waiver = string.Empty;
                }

                _distributorDocument.UpdateAsync(distributerObj, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, "Document deleted successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post/DeleteDocumentFle");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpGet]
        public IActionResult NewSubscriptionActivation(long id = 0)
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            var subscriptionType = _subscriptionType.GetAll(x => x.IsActive).Select(x => new SubscriptionTypeDto
            {
                Id = x.Id,
                SubscriptionTypeName = x.SubscriptionTypeName,
                ChargedMonthly = x.ChargedMonthly,
                SubscriptionCharge = x.SubscriptionCharge,
                Description = x.Description,
                Brand = x.Brand,
                Generic = x.Generic,
                Otc = x.Otc
            }).ToList();
            var model = new DistributorSubscriptionDto
            {
                Id = id,
                SubscriptionTypeId = 0,
                SubscriptionTypeDtos = subscriptionType
            };
            //ViewBag.DistributorList = _distributor.GetDistributorAdminList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).ToList();
            model.DistributorId = model.DistributorId == 0 ? distributorId : model.DistributorId;
            model.Email = _distributor.GetById(model.DistributorId).DistributorAdminUser.Email;
            if (id == 0) return View(model);
            model = Mapper.Map<DistributorSubscriptionDto>(_distributorSubscription.GetSingle(x => x.Id == id));
            model.SubscriptionTypeDtos = subscriptionType;
            model.DateStart = model.StartDate.ToDefaultDateTime(GlobalFormates.DefaultDate);
            return View(model);
        }

        [HttpGet]
        public IActionResult UpgradSubscription(long id, string subscriptionTypeId, string startDate)
        {
            var sTypeId = string.IsNullOrEmpty(subscriptionTypeId) ? 0 : Convert.ToInt64(subscriptionTypeId);
            var subscriptionType = _subscriptionType.GetAll(x => x.IsActive).Select(x => new SubscriptionTypeDto
            {
                Id = x.Id,
                SubscriptionTypeName = x.SubscriptionTypeName,
                ChargedMonthly = x.ChargedMonthly,
                SubscriptionCharge = x.SubscriptionCharge,
                Description = x.Description,
                Brand = x.Brand,
                Generic = x.Generic,
                Otc = x.Otc
            }).ToList();
            var model = new DistributorSubscriptionDto
            {
                SubscriptionTypeId = sTypeId,
                SubscriptionTypeDtos = subscriptionType,
                DistributorId = id,
                DateStart = startDate
            };
            //ViewBag.DistributorList = _distributor.GetDistributorAdminList().Where(x => x.Value == model.DistributorId.ToString()).Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString() }).OrderBy(x => x.Text).ToList();

            var distributor = _distributor.GetById(model.DistributorId);
            model.Email = distributor.DistributorAdminUser.Email;
            ViewBag.DistributorList = _distributor.GetAll().Where(x => x.Id == model.DistributorId).Select(x => new SelectListItem { Text = x.CompanyName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            if (id == 0) return View(model);
            var distributorSubObj = _distributorSubscription.GetSingle(x => x.DistributorId == model.DistributorId);

            model = Mapper.Map<DistributorSubscriptionDto>(_distributorSubscription.GetSingle(x => x.DistributorId == model.DistributorId));
            model.Notes = distributorSubObj.Notes;
            model.Email = distributor.Email;
            model.SubscriptionTypeDtos = subscriptionType;
            model.DateStart = string.IsNullOrEmpty(startDate) ? model.StartDate.ToDefaultDateTime(GlobalFormates.DefaultDate) : startDate;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpgradSubscription(DistributorSubscriptionDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (model.Id == 0)
                        {
                            var activeSubscription = _distributorSubscription.GetSingle(x => x.DistributorId == model.DistributorId && x.IsActive);
                            if (activeSubscription?.EndDate != null)
                            {
                                if (activeSubscription.EndDate.Value <= DateTime.Today)
                                    activeSubscription.IsUpgraded = true;
                                else
                                    activeSubscription.IsActive = false;
                                await _distributorSubscription.UpdateAsync(activeSubscription, Accessor, User.GetUserId());
                            }
                        }

                        var result = _distributorSubscription.GetSingle(x => x.Id == model.Id);
                        if (result != null)
                        {
                            if (result.SubscriptionTypeId != model.SubscriptionTypeId || result.SubscriptionTypeId > model.SubscriptionTypeId)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Distributor subsription cannot be same or downgraded !");
                            }
                            result.IsActive = true;
                            result.StartDate = Convert.ToDateTime(model.DateStart);
                            result.EndDate = result.StartDate.AddMonths(1);
                            result.SubscriptionTypeId = model.SubscriptionTypeId;
                            result.Notes = model.Notes;
                        }
                        await _distributorSubscription.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Distributor subscription updated successfully.");
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/NewSubscriptionActivation");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SetSessionNewSubscriptionActivation(DistributorSubscriptionDto model)
        {
            try
            {
                DistributorSubscriptionDto sessionModel;
                var result = _distributorSubscription.GetSingle(x => x.DistributorId == model.DistributorId);
                if (result != null)
                {
                    if (result.SubscriptionTypeId == model.SubscriptionTypeId || result.SubscriptionTypeId > model.SubscriptionTypeId)
                        return JsonResponse.GenerateJsonResult(0, "Distributor subsription can't be same or downgraded !");

                    sessionModel = Accessor.HttpContext.Session.GetObjectFromJson<DistributorSubscriptionDto>("NewSubscriptionActivation");
                    if (sessionModel == null)
                    {
                        Accessor.HttpContext.Session.SetObjectAsJson("NewSubscriptionActivation", model);
                        return JsonResponse.GenerateJsonResult(1);
                    }
                }
                if (model.SubscriptionTypeId == 0)
                    return JsonResponse.GenerateJsonResult(0, "Please select Distributor Subscription Plan.");
                sessionModel = Accessor.HttpContext.Session.GetObjectFromJson<DistributorSubscriptionDto>("NewSubscriptionActivation");
                if (sessionModel != null) return JsonResponse.GenerateJsonResult(1);
                Accessor.HttpContext.Session.SetObjectAsJson("NewSubscriptionActivation", model);
                return JsonResponse.GenerateJsonResult(1);
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "SetSessionNewSubscriptionActivation");
                return JsonResponse.GenerateJsonResult(0);
            }
        }

        public async Task<IActionResult> PaymentWithPaypal(string subscriptionName, string subscriptionAmount)
        {
            //getting the apiContext
            var apiContext = _payPal.GetApiContext();
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //A resource representing a Payer that funds a payment Payment Method as paypal
                    //Payer Id will be returned when payment proceeds or click to pay

                    //string payerId = Request.Params["PayerID"];
                    var payerId = HttpContext.Request.Query["PayerID"].ToString();

                    if (string.IsNullOrEmpty(payerId))
                    {
                        //this section will be executed first because PayerID doesn't exist it is returned by the create function call of the payment class

                        // Creating a payment
                        // baseURL is the url on which paypal sendsback the data.

                        var baseUri = Accessor.HttpContext.Request.GetDisplayUrl();

                        //here we are generating guid for storing the paymentID received in session
                        //which will be used in the payment execution

                        var guid = Convert.ToString(new Random().Next(100000));

                        //CreatePayment function gives us the payment approval url on which payer is redirected for paypal account payment

                        var createdPayment = CreatePayment(apiContext, subscriptionName, subscriptionAmount, baseUri + "&guid=" + guid);

                        //get links returned from paypal in response to Create function call

                        using (var links = createdPayment.links.GetEnumerator())
                        {
                            string paypalRedirectUrl = null;
                            while (links.MoveNext())
                            {
                                // Do something with value and enumerator.Current
                                Links lnk = links.Current;
                                if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                                {
                                    //saving the payapalredirect URL to which user will be redirected for payment
                                    paypalRedirectUrl = lnk.href;
                                }
                            }
                            // Saving the paymentID in the key guid
                            Session.SetString(guid, createdPayment.id);
                            return Redirect(paypalRedirectUrl);
                        }
                    }
                    else
                    {
                        // This function exectues after receving all parameters for the payment
                        var guid = HttpContext.Request.Query["guid"];

                        var executedPayment = ExecutePayment(apiContext, payerId, Session.GetString(guid));

                        ////If executed payment failed then we will show payment failure message to user
                        if (executedPayment.state.ToLower() != "approved")
                        {
                            return View("FailureView");
                        }
                        var model = Accessor.HttpContext.Session.GetObjectFromJson<DistributorSubscriptionDto>("NewSubscriptionActivation");
                        if (model == null) return View("FailureView");
                        if (model.Id == 0)
                        {
                            var subscriptionType = _subscriptionType.GetSingle(x => x.Id == model.SubscriptionTypeId);
                            //  var startDate = Convert.ToDateTime(model.DateStart);
                            var startDate = Convert.ToDateTime(DateTime.UtcNow.Date);
                            var distributerSubscription = new DistributorSubscription
                            {
                                DistributorId = model.DistributorId,
                                Notes = model.Notes,
                                StartDate = startDate,
                                EndDate = startDate.AddMonths(GetDefaultSubscriptionPlanDuration()),
                                SubscriptionTypeId = model.SubscriptionTypeId,
                                ChargedMonthly = subscriptionType.ChargedMonthly,
                                SubscriptionCharge = subscriptionType.SubscriptionCharge,
                                Brand = subscriptionType.Brand,
                                Generic = subscriptionType.Generic,
                                Otc = subscriptionType.Otc,
                                IsActive = true,
                                IsPayment = true,
                                PaymentDate = DateTime.UtcNow,
                                PayPalTransactionId = executedPayment.id
                            };
                            await _distributorSubscription.InsertAsync(distributerSubscription, Accessor, User.GetUserId());
                            var distributorUser = _distributor.GetById(model.DistributorId).DistributorAdminUser;
                            #region Send Mail After Subscription Plan Payment
                            var subscriptionTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.Subscription, GetPhysicalUrl());
                            subscriptionTemplate = subscriptionTemplate.Replace("{UserName}", distributorUser.FullName);
                            await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                            {
                                ToAddress = distributorUser.Email,
                                Subject = "Subscription Activation",
                                BodyText = subscriptionTemplate,
                                ToDisplayName = distributorUser.FullName
                            });
                            #endregion
                            txscope.Complete();
                            await _signInManager.SignOutAsync();
                            return RedirectToAction(nameof(AccountController.Login), "Account");
                        }
                        var result = _distributorSubscription.GetSingle(x => x.DistributorId == model.Id);

                        if (result == null) return View("FailureView");
                        if (result.SubscriptionTypeId == model.SubscriptionTypeId || result.SubscriptionTypeId > model.SubscriptionTypeId)
                        {
                            txscope.Dispose();
                            return View("FailureView");
                        }

                        //Insert Old Subscription Details into distributorsubscriptionHistory Table

                        DistributorSubscriptionHistory dsh = new DistributorSubscriptionHistory
                        {
                            ChargedMonthly = result.ChargedMonthly,
                            Brand = result.Brand,
                            Generic = result.Generic,
                            Otc = result.Otc,
                            IsActive = result.IsActive,
                            StartDate = result.StartDate,
                            EndDate = result.EndDate,
                            SubscriptionTypeId = result.SubscriptionTypeId,
                            Notes = result.Notes,
                            IsPayment = result.IsPayment,
                            PaymentDate = result.PaymentDate,
                            PayPalTransactionId = result.PayPalTransactionId,
                            DistributorId = result.DistributorId
                        };
                        await _distributorSubscriptionHistory.InsertAsync(dsh, Accessor, User.GetUserId());



                        result.IsActive = true;
                        result.StartDate = Convert.ToDateTime(model.DateStart);
                        result.EndDate = result.StartDate.AddMonths(12);
                        result.SubscriptionTypeId = model.SubscriptionTypeId;
                        result.Notes = model.Notes;
                        result.IsPayment = true;
                        result.PaymentDate = DateTime.UtcNow;
                        result.PayPalTransactionId = executedPayment.id;
                        var UpdatedSubscription = await _distributorSubscription.UpdateAsync(result, Accessor, User.GetUserId());

                        txscope.Complete();
                        await _signInManager.SignOutAsync();
                        return Redirect("/Home/SuccessView");
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "PaymentWithPaypal");
                    return View("FailureView");
                }
            }
        }

        #endregion

        #region Common     
        private Payment CreatePayment(APIContext apiContext, string subscriptionName, string subscriptionAmount, string redirectUrl)
        {
            //create itemlist and add item objects to it
            var itemList = new ItemList { items = new List<Item>() };

            //Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item
            {
                name = subscriptionName,
                currency = "USD",
                price = subscriptionAmount,
                quantity = "1",
                sku = "sku-" + subscriptionName
            });

            var payer = new Payer { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details
            var details = new Details { tax = subscriptionAmount };

            //Final amount with details
            var amount = new Amount
            {
                currency = "USD",
                total = subscriptionAmount, // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<PayPal.Api.Transaction>
            {
                new PayPal.Api.Transaction
                {
                    description= "Transaction description",
                    invoice_number = CommonMethod.CreateRandomStringByLength(), //Generate an Invoice No
                    amount = amount,
                    item_list = itemList
                }
            };
            // Adding description about the transaction

            _payment = new Payment
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext
            return _payment.Create(apiContext);
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution { payer_id = payerId };
            _payment = new Payment { id = paymentId };
            return _payment.Execute(apiContext, paymentExecution);
        }

        private void BindDropdownList()
        {
            ViewBag.StateList = _state.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}