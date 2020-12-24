using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;

namespace RxFair.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController<AccountController>
    {
        private readonly JsonResponse _jsonResponse;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly EmailService _emailService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IErrorLogService _errorLog;
        private readonly IDistributorService _distributer;
        private readonly IMedicineMasterService _medicine;
        private readonly IPharmacyService _pharmacy;
        private readonly IUserService _user;
        private readonly IUploadedMedicineService _uploadedMedicine;
        private readonly ILogger _logger;
        private ISession Session => Accessor.HttpContext.Session;
        private readonly IRolesModuleAccessService _rolesModuleAccess;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<Role> roleManager, IOptions<EmailSettingsGmail> emailSettingsGmail,
            IHostingEnvironment hostingEnvironment, IDistributorService distributer, IMedicineMasterService medicine, IPharmacyService pharmacy, IUserService user, IUploadedMedicineService uploadedMedicine,
            IErrorLogService errorLog, ILogger<AccountController> logger, IRolesModuleAccessService rolesModuleAccess)
        {
            _jsonResponse = new JsonResponse();
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = new EmailService(emailSettingsGmail);
            _hostingEnvironment = hostingEnvironment;
            _errorLog = errorLog;
            _distributer = distributer;
            _medicine = medicine;
            _pharmacy = pharmacy;
            _user = user;
            _uploadedMedicine = uploadedMedicine;
            _logger = logger;
            _rolesModuleAccess = rolesModuleAccess;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
                var userRole = GetUserRoles(user).FirstOrDefault();
                switch (CommonMethod.GetUserGroupName(userRole))
                {
                    case UserRoleGroup.Developer:
                        return RedirectToAction("SystemModule", "Master", new { area = "Admin" });
                    case UserRoleGroup.Admin:
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Admin))
                            return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    case UserRoleGroup.Distributor:
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Distributor))
                            return RedirectToLocal(returnUrl);
                        else
                        {
                            if (userRole == null || !userRole.Equals(UserRoles.DistributorPrimaryAdmin)) return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });
                            if (user.Distributor?.DistributorSubscriptions.Count(x => x.IsActive) == 0)
                                return RedirectToAction("NewSubscriptionActivation", "MyAccount", new { area = "Distributor" });
                            return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });
                        }
                    case UserRoleGroup.Pharmacy:
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Pharmacy))
                            return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Dashboard", new { area = "Pharmacy" });
                    default:
                        return RedirectToLocal(returnUrl);
                }
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", GlobalConstant.InvalidLogin);
                return View(model);
            }

            var userRole = GetUserRoles(user).FirstOrDefault();
            if (!userRole.NullToString().Equals(UserRoles.SystemSuperAdmin))
            {
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, GlobalConstant.ConfirmEmail);
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, GlobalConstant.AccountDeactivated);
                    return View(model);
                }
            }
            if (userRole != null && userRole.Contains(UserRoleGroup.Pharmacy))
            {
                if (user.Pharmacy == null)
                {
                    if (userRole.Equals(UserRoles.PharmacyPrimaryAdmin)) return RedirectToLocal(Url.AfterPharmacyEmailConfirm(user.Id, Request.Scheme));
                    ModelState.AddModelError(string.Empty, @"Pharmacy does not exist.");
                    return View(model);
                }

                if (!user.Pharmacy.IsActive)
                {
                    ModelState.AddModelError(string.Empty, GlobalConstant.AccountDeactivated);
                    return View(model);
                }
                if (user.Pharmacy.Status != 2)
                {
                    ModelState.AddModelError(string.Empty, GlobalConstant.PharmacyNotAccepted);
                    return View(model);
                }
            }
            if (userRole != null && userRole.Contains(UserRoleGroup.Distributor))
            {
                if (user.Distributor == null)
                {
                    if (!userRole.Equals(UserRoles.DistributorPrimaryAdmin))
                        ModelState.AddModelError(string.Empty, @"Distributor does not exist.");
                    else
                        return RedirectToLocal(Url.AfterDistributorEmailConfirm(user.Id, Request.Scheme));
                }
                else if (!user.Distributor.IsActive)
                {
                    ModelState.AddModelError(string.Empty, GlobalConstant.AccountDeactivated);
                    return View(model);
                }
                else if (!userRole.Equals(UserRoles.DistributorPrimaryAdmin))
                {
                    if (user.Distributor?.DistributorSubscriptions.Count(x => x.IsActive && x.EndDate.Value.Date > DateTime.Now.Date) == 0)
                    {
                        ModelState.AddModelError(string.Empty, GlobalConstant.DistributorSubscriptionExpired);
                        return View(model);
                    }
                }
            }
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                user.LastLogin = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);

                List<RolesModuleAccessDto> accessAllMenu;
                switch (CommonMethod.GetUserGroupName(userRole))
                {
                    case UserRoleGroup.Developer:
                        return RedirectToAction("SystemModule", "Master", new { area = "Admin" });
                    case UserRoleGroup.Admin:
                        accessAllMenu = RoleWishPermission(user.Id);
                        Accessor.HttpContext.Session.SetObjectAsJson("AllMenu", accessAllMenu);
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Admin))
                            return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    case UserRoleGroup.Distributor:
                        accessAllMenu = RoleWishPermission(user.Id);
                        Accessor.HttpContext.Session.SetObjectAsJson("AllMenu", accessAllMenu);
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Distributor))
                            return RedirectToLocal(returnUrl);
                        else
                        {
                            if (userRole == null || !userRole.Equals(UserRoles.DistributorPrimaryAdmin)) return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });
                            if (user.Distributor?.DistributorSubscriptions.Count(x => x.IsActive && x.EndDate.Value.Date > DateTime.Now.Date) == 0)
                                return RedirectToAction("NewSubscriptionActivation", "MyAccount", new { area = "Distributor" });
                            if (string.IsNullOrEmpty(user.FullName.Trim()))
                                return RedirectToAction("Index", "MyAccount", new { area = "Distributor" });
                            return RedirectToAction("Index", "Dashboard", new { area = "Distributor" });
                        }
                    case UserRoleGroup.Pharmacy:
                        accessAllMenu = RoleWishPermission(user.Id);
                        Accessor.HttpContext.Session.SetObjectAsJson("AllMenu", accessAllMenu);
                        if (returnUrl.NullToString().Contains(UserRoleGroup.Pharmacy))
                            return RedirectToLocal(returnUrl);
                        return RedirectToAction("Index", "Dashboard", new { area = "Pharmacy" });
                    default:
                        return RedirectToLocal(returnUrl);
                }
            }
            if (result.RequiresTwoFactor)
                return RedirectToAction(nameof(LoginWith2Fa), new { returnUrl, model.RememberMe });
            if (result.IsLockedOut)
                return RedirectToAction(nameof(Lockout));

            ModelState.AddModelError(string.Empty, GlobalConstant.InvalidLogin);
            return View(model);

            // If we got this far, something failed, redisplay form
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2Fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2FaViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2Fa(LoginWith2FaViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel()
                    {
                        ToAddress = model.Email,
                        BodyText = callbackUrl
                    });

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Login", "Account", new { returnUrl });
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            if (user.EmailConfirmed)
            {
                TempData["Message"] = @"Your email is already confirmed.";
                return RedirectToAction("ConfirmEmailConfirmation", "Account");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded) return View("Error");
            var userRole = GetUserRoles(user).FirstOrDefault();
            if (userRole == null || (!userRole.Equals(UserRoles.PharmacyPrimaryAdmin) && !userRole.Equals(UserRoles.DistributorPrimaryAdmin)))
            {
                TempData["Message"] = @"Your email is confirmed.";
                return RedirectToAction("ConfirmEmailConfirmation", "Account");
            }
            var returnUrl = GroupWiseUser(user.Id, userRole);
            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("ConfirmEmailConfirmation", "Account");
            if (returnUrl.Equals("/Account/Login"))
                return RedirectToAction("ConfirmEmailConfirmation", "Account");
            return Redirect(returnUrl);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmEmailConfirmation()
        {
            TempData.Keep("Message");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Message"] = GlobalConstant.EmailNotFound;
                return RedirectToAction(nameof(ForgotPassword));
            }
            if (!(await _userManager.IsEmailConfirmedAsync(user)))
            {
                TempData["Message"] = GlobalConstant.ConfirmEmail;
                return RedirectToAction(nameof(ForgotPassword));
            }

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
            string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLog, _hostingEnvironment.WebRootPath, EmailTemplateFileList.ResetPassword, GetPhysicalUrl());
            emailTemplate = emailTemplate.Replace("{UserName}", user.FullName);
            emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);

            await _emailService.SendEmailAsyncByGmail(new SendEmailModel
            {
                ToAddress = user.Email,
                Subject = "Reset Password",
                BodyText = emailTemplate
            });

            TempData["Message"] = @"Please check your email to reset your password.";
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }

            var userId = Accessor.HttpContext.Request.Query["userId"];
            var user = await _userManager.FindByIdAsync(userId);
            var model = new ResetPasswordViewModel { Code = code, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                TempData["Message"] = @"Your password does not reset successfully.";
                return RedirectToAction(nameof(Login));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                TempData["Message"] = @"Your password reset successfully.";
                return RedirectToAction(nameof(Login));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.HideLogin = true;
            return View();
        }

        #region Global Methods
        public async Task<IActionResult> AddEditUser(long id, string userGroup)
        {
            BindDropdownList(userGroup);
            ViewBag.GroupName = userGroup == UserRoleGroup.Admin ? "System" : userGroup;
            if (id == 0) return View(@"Components/_AddEditUser", new UserBasicInfo { Id = id, UserGroup = userGroup });
            var result = await _userManager.FindByIdAsync(id.ToString());
            var tempView = new UserBasicInfo
            {
                Id = result.Id,
                FirstName = result.FirstName,
                Email = result.Email,
                LastName = result.LastName,
                Mobile = result.Mobile,
                Role = GetUserRoles(result).FirstOrDefault(),
                UserGroup = userGroup
            };
            return View(@"Components/_AddEditUser", tempView);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditUser(UserBasicInfo model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    long userId = User.GetUserId();
                    var currentPrimaryAdmin = await _userManager.FindByIdAsync(userId.ToString());
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }
                    if (model.IsRoleChange)
                    {
                        string removeRole = string.Empty, newRole = string.Empty;

                        switch (model.UserGroup)
                        {
                            case UserRoleGroup.Admin:
                                removeRole = UserRoles.SystemSuperAdmin;
                                newRole = UserRoles.SystemSuperAdmin;
                                break;
                            case UserRoleGroup.Pharmacy:
                                removeRole = UserRoles.PharmacyPrimaryAdmin;
                                newRole = UserRoles.PharmacyAdmin;
                                break;
                            case UserRoleGroup.Distributor:
                                removeRole = UserRoles.DistributorPrimaryAdmin;
                                newRole = UserRoles.DistributorAdmin;
                                break;
                        }

                        await _userManager.RemoveFromRoleAsync(currentPrimaryAdmin, removeRole);
                        await _userManager.AddToRoleAsync(currentPrimaryAdmin, newRole);
                    }
                    if (model.Id == 0)
                    {
                        var isExist = await _userManager.FindByEmailAsync(model.Email);
                        if (isExist != null)
                        {
                            txscope.Dispose();
                        }
                        var newUser = new ApplicationUser
                        {
                            Id = model.Id,
                            FirstName = model.FirstName,
                            Email = model.Email,
                            UserName = model.Email,
                            LastName = model.LastName,
                            Mobile = model.Mobile,
                            IsActive = true,
                        };
                        string password = CommonMethod.CreateRandomPassword(8);
                        await _userManager.CreateAsync(newUser, password);
                        await _userManager.AddToRoleAsync(newUser, model.Role);

                        // get user roles
                        if (!User.IsInRole(UserRoles.SystemSuperAdmin))
                        {
                            switch (model.UserGroup)
                            {
                                case UserRoleGroup.Pharmacy:
                                    newUser.PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                                    if (model.Role.Contains("PrimaryAdmin"))
                                        currentPrimaryAdmin.Pharmacy.UserId = newUser.Id;
                                    break;
                                case UserRoleGroup.Distributor:
                                    newUser.DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                                    if (model.Role.Contains("PrimaryAdmin"))
                                    {
                                        currentPrimaryAdmin.Distributor.UserId = newUser.Id;
                                        currentPrimaryAdmin.Distributor.Email = newUser.Email;
                                    }
                                    break;
                            }

                            await _userManager.UpdateAsync(currentPrimaryAdmin);
                        }
                        await _userManager.UpdateAsync(newUser);
                        txscope.Complete();

                        #region Send Email

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                        var callbackUrl = Url.EmailConfirmationLink(newUser.Id, code, Request.Scheme);
                        string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.NewUser, GetPhysicalUrl());
                        emailTemplate = emailTemplate.Replace("{UserName}", newUser.FullName);
                        emailTemplate = emailTemplate.Replace("{Password}", password);
                        emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);
                        string subject = $@"User Created Successfully with Role {CommonMethod.GetDisplayUserRole(model.Role)}";
                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                        {
                            Subject = subject,
                            ToAddress = newUser.Email,
                            ToDisplayName = newUser.FullName,
                            BodyText = emailTemplate
                        });

                        #endregion

                        return JsonResponse.GenerateJsonResult(1, $@"{(model.UserGroup == "Admin" ? "System" : model.UserGroup)} User inserted successfully.", newUser.Id);
                    }
                    var result = await _userManager.FindByIdAsync(model.Id.ToString());
                    var currentRole = _roleManager.FindByNameAsync(model.Role);
                    result.FirstName = model.FirstName;
                    result.Email = model.Email;
                    result.LastName = model.LastName;
                    result.Mobile = model.Mobile;
                    await _userManager.UpdateAsync(result);
                    await _user.UpdateAsync(result, Accessor, userId);
                    var userRole = GetUserRoles(result).FirstOrDefault();
                    if (userRole != null && !userRole.Equals(model.Role))
                    {
                        await _userManager.RemoveFromRoleAsync(result, userRole);
                        await _userManager.AddToRoleAsync(result, model.Role);
                        // get user roles
                        if (!User.IsInRole(UserRoles.SystemSuperAdmin))
                        {
                            switch (model.UserGroup)
                            {
                                case UserRoleGroup.Pharmacy:
                                    result.PharmacyId = Convert.ToInt64(User.GetClaimValue(UserClaims.PharmacyId));
                                    if (model.Role.Contains("PrimaryAdmin"))
                                        currentPrimaryAdmin.Pharmacy.UserId = result.Id;
                                    break;
                                case UserRoleGroup.Distributor:
                                    result.DistributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
                                    if (model.Role.Contains("PrimaryAdmin"))
                                    {
                                        currentPrimaryAdmin.Distributor.UserId = result.Id;
                                        currentPrimaryAdmin.Distributor.Email = result.Email;
                                    }
                                    break;
                            }
                            await _userManager.UpdateAsync(currentPrimaryAdmin);
                        }

                        //tva
                        string emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.UpdateRole, GetPhysicalUrl());
                        emailTemplate = emailTemplate.Replace("{UserName}", result.FullName);
                        emailTemplate = emailTemplate.Replace("{UserRole}", CommonMethod.GetDisplayUserRole(model.Role));
                        
                        string subject = $@"User Role updated successfully.";

                        await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                        {
                            Subject = subject,
                            ToAddress = result.Email,
                            ToDisplayName = result.FullName,
                            BodyText = emailTemplate
                        });
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"{(model.UserGroup == "Admin" ? "System" : model.UserGroup)} User updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditUser");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveUser(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var result = await _userManager.FindByIdAsync(id.ToString());
                    if (result != null)
                    {
                        var userRole = GetUserRoles(result).FirstOrDefault();
                        if (userRole != null && userRole.Contains("PrimaryAdmin"))
                        {
                            if (CommonMethod.GetUserGroupName(userRole) == UserRoleGroup.Pharmacy && result.Pharmacy != null)
                            {
                                result.Pharmacy.IsActive = false;
                                await _pharmacy.UpdateAsync(result.Pharmacy, Accessor, User.GetUserId());
                            }
                            else if (CommonMethod.GetUserGroupName(userRole) == UserRoleGroup.Distributor && result.Pharmacy != null)
                            {
                                result.Distributor.IsActive = false;
                                await _distributer.UpdateAsync(result.Distributor, Accessor, User.GetUserId());
                            }
                        }
                        result.IsActive = false;
                        // await _userManager.RemoveFromRoleAsync(result, userRole);
                        await _userManager.UpdateAsync(result);

                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, @"User deleted successfully.");
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveUser");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CheckUserIsExist(string email)
        {
            email = email.Trim();
            var isExist = _user.GetSingle(x => x.UserName.Equals(email) || x.UserName.Equals(email) || x.NormalizedEmail.Equals(email));
            return JsonResponse.GenerateJsonResult(isExist != null ? 1 : 0, isExist != null ? GlobalConstant.AlreadyRegisterd : "");
        }

        [HttpGet]
        public async Task<IActionResult> GlobalResetPassword(long id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                var role = GetUserRoles(user).FirstOrDefault();
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return _jsonResponse.GenerateJsonResult(0, "Don't reveal that the user is not confirmed");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                string emailTemplate = CommonMethod.ReadEmailTemplate(_errorLog, _hostingEnvironment.WebRootPath, EmailTemplateFileList.ResetPassword, GetPhysicalUrl());
                emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);
                emailTemplate = emailTemplate.Replace("{UserName}", user.FullName);
                await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                {
                    ToAddress = user.Email,
                    Subject = "Reset Password",
                    BodyText = emailTemplate
                });
                var UserType = CommonMethod.GetUserGroupName(role) == "Admin" ? "System" : CommonMethod.GetUserGroupName(role);
                return _jsonResponse.GenerateJsonResult(1, $@"{UserType}  user password link sent successfully");
            }
            catch (Exception ex)
            {
                _errorLog.AddErrorLog(ex, "GET/ResetPassword");
                return _jsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }

        [HttpGet]
        public IActionResult CheckFileExist(string url)
        {
            if (string.IsNullOrEmpty(url))
                return JsonResponse.GenerateJsonResult(0, "File is missing !");

            var path = Path.Combine("wwwroot", url);

            return System.IO.File.Exists(path)
                ? JsonResponse.GenerateJsonResult(1, "File is available.", url)
                : JsonResponse.GenerateJsonResult(0, "File is missing !");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string url)
        {
            if (url == null)
                return Content("File name not present !");
            var path = Path.Combine("wwwroot", url);

            //if (System.IO.File.Exists(path))
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, CommonMethod.GetContentType(url), Path.GetFileName(url));
        }

        [HttpGet]
        public async Task<IActionResult> ViewMedicine(long id, bool isUploaded)
        {
            var distributorId = CommonMethod.GetUserGroupName(User.GetClaimValue(UserClaims.UserRole)).Contains(UserRoleGroup.Distributor) ? Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId)) : 0;
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.BigInt) { Value = id },
                new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = distributorId },
                new SqlParameter("@IsUploaded", SqlDbType.Bit) { Value = isUploaded }
            };
            var model = await _medicine.GetViewMedicineInfo(parameters.ToArray());
            if (!string.IsNullOrEmpty(model?.MedicineImage))
            {
                //model.MedicineImage = $@"/{FilePathList.MedicineImage}/{model.MedicineImage}";
                model.MedicineImage = GetS3ServiceUrl(BucketName.MedicineImage, model.MedicineImage);
            }
            return View(@"Components/_ViewMedicine", model);
        }

        private List<RolesModuleAccessDto> RoleWishPermission(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", SqlDbType.BigInt) { Value = id }
            };
            var rolePermissionAccess = _rolesModuleAccess.RolesModuleAccess(parameters.ToArray());
            return rolePermissionAccess;
        }
        #endregion
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public string GroupWiseUser(long userId, string userRole)
        {
            switch (CommonMethod.GetUserGroupName(userRole))
            {
                case UserRoleGroup.Distributor:
                    if (_distributer.GetSingle(x => x.UserId == userId) != null)
                    {
                        TempData["MessageType"] = 1;
                        TempData["Message"] = @"Your email is confirmed.";
                        return "/Account/Login";
                    }
                    else
                    {
                        TempData["Message"] = @"Please fill up the required information.";
                        return Url.AfterDistributorEmailConfirm(userId, Request.Scheme);
                    }
                case UserRoleGroup.Pharmacy:
                    if (_pharmacy.GetSingle(x => x.UserId == userId) != null)
                    {
                        TempData["MessageType"] = 1;
                        TempData["Message"] = @"Your email is confirmed.";
                        return "/Account/Login";
                    }
                    else
                    {
                        TempData["Message"] = @"Please fill up the required information.";
                        return Url.AfterPharmacyEmailConfirm(userId, Request.Scheme);
                    }
                default:
                    return "/Account/Login";
            }
        }

        public IList<string> GetUserRoles(ApplicationUser user)
        {
            var rolesList = Task.Run(() => _userManager.GetRolesAsync(user)).Result;
            return rolesList;
        }

        public void BindDropdownList(string userGroup)
        {
            var roleList = new List<long>();
            switch (userGroup)
            {
                case UserRoleGroup.Admin:
                    roleList = new List<long> { 2, 3, 4, 5, 6 };
                    break;
                case UserRoleGroup.Distributor:
                    roleList = User.IsInRole(UserRoles.SystemSuperAdmin) ? new List<long> { 7 } : new List<long> { 7, 8, 9 };
                    break;
                case UserRoleGroup.Pharmacy:
                    roleList = User.IsInRole(UserRoles.SystemSuperAdmin) ? new List<long> { 10 } : new List<long> { 10, 11, 12 };
                    break;
            }

            ViewBag.RoleList = _roleManager.Roles.Where(x => roleList.Contains(x.Id)).Select(x => new SelectListItem()
            {
                Text = x.DisplayRoleName,
                Value = x.Name
            }).OrderBy(x => x.Text);
        }
        #endregion
    }
}
