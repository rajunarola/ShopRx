using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Service.Utility;
using RxFair.Utility;
using RxFair.Utility.Common;
using RxFair.Utility.Extension;
using RxFair.Utility.Helpers;
using DbDistributor = RxFair.Data.DbModel.Distributor;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize, Area("Admin")]
    public class MasterController : BaseController<MasterController>
    {
        #region Fields

        private readonly IAdvertiseTemplateService _advertiseTemplate;
        private readonly IContactUsService _contactUs;
        private readonly IContactRequestService _contactRequest;
        private readonly IDistributorService _distributer;
        private readonly IDistributerOrderSettingService _distributorSetting;
        private readonly IDocumentService _document;
        private readonly IDosageFormService _dosageForm;
        private readonly EmailService _emailService;
        private readonly IFaQsService _faQs;
        private readonly IManufacturerService _manufacturer;
        private readonly IMeasurementService _measurement;
        private readonly IMedicineCategoryService _medicineCategory;
        private readonly INewDistributorRequestService _newDistributor;
        private readonly ITestimonialsService _testimonials;
        private readonly ITermsAndConditionService _termsAndCondition;
        private readonly IRolesModuleAccessService _rolesModuleAccess;
        private readonly ISystemModuleService _systemModule;
        private readonly IAccessModuleFunctionalityService _accessModuleFunctionality;
        private readonly IFunctionalityService _functionality;
        private readonly ISubscriptionTypeService _subscriptionType;
        private readonly IUserService _user;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributorSubscriptionService _distributorSubscription;
        #endregion

        #region Ctor
        public MasterController(IDistributorSubscriptionService distributorSubscription, IAdvertiseTemplateService advertiseTemplateService, IContactUsService contactUs, IContactRequestService contactRequest, IDistributorService distributer, IDocumentService document, IDosageFormService dosageForm, IOptions<EmailSettingsGmail> emailSettingsGmail, IFaQsService faQs,
            IManufacturerService manufacturer, IMeasurementService measurement, IMedicineCategoryService medicineCategory, INewDistributorRequestService newDistributor, ITestimonialsService testimonials, ITermsAndConditionService termsAndCondition,
            IRolesModuleAccessService rolesModuleAccess, ISystemModuleService systemModule, ISubscriptionTypeService subscriptionType, IUserService user, UserManager<ApplicationUser> userManager, IDistributerOrderSettingService distributorSetting, IAccessModuleFunctionalityService accessModuleFunctionality, IFunctionalityService functionality)
        {
            _advertiseTemplate = advertiseTemplateService;
            _contactUs = contactUs;
            _contactRequest = contactRequest;
            _distributer = distributer;
            _document = document;
            _dosageForm = dosageForm;
            _emailService = new EmailService(emailSettingsGmail);
            _faQs = faQs;
            _manufacturer = manufacturer;
            _measurement = measurement;
            _medicineCategory = medicineCategory;
            _newDistributor = newDistributor;
            _testimonials = testimonials;
            _termsAndCondition = termsAndCondition;
            _rolesModuleAccess = rolesModuleAccess;
            _systemModule = systemModule;
            _subscriptionType = subscriptionType;
            _user = user;
            _userManager = userManager;
            _distributorSetting = distributorSetting;
            _accessModuleFunctionality = accessModuleFunctionality;
            _functionality = functionality;
            _distributorSubscription = distributorSubscription;
        }
        #endregion

        #region Methods

        #region Access Permission
        [Route("AccessPermission")]
        public IActionResult AccessPermission()
        {
            var userRoleGroup = User.GetClaimValue(UserClaims.UserRoleGroup);
            if (userRoleGroup.Equals(UserRoleGroup.Admin) || userRoleGroup.Equals(UserRoleGroup.Developer))
            {
                ViewBag.UserGroup = EnumHelpers.EnumToList<GlobalEnums.UserGroup>().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Value.ToString()
                }).OrderBy(x => x.Text).ToList();
            }
            if (userRoleGroup.Equals(UserRoleGroup.Distributor))
            {
                ViewBag.UserGroup = EnumHelpers.EnumToList<GlobalEnums.UserGroup>().Where(x => x.Name.Equals(UserRoleGroup.Distributor))
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Value.ToString()
                    }).OrderBy(x => x.Text).ToList();
            }
            if (userRoleGroup.Equals(UserRoleGroup.Pharmacy))
            {
                ViewBag.UserGroup = EnumHelpers.EnumToList<GlobalEnums.UserGroup>().Where(x => x.Name.Equals(UserRoleGroup.Pharmacy))
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Value.ToString()
                    }).OrderBy(x => x.Text).ToList();
            }
            return View();
        }

        public async Task<IActionResult> RoleWishPermission(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserGroupId", SqlDbType.Int) { Value = id }
            };
            var userRole = await _user.GetUserGroupWishRole(parameters.ToArray());
            var roleList = userRole.Select(x => new RolesViewModel
            {
                Id = x.Id,
                RoleName = x.Name
            }).ToList();
            var allMenu = new AllMenu
            {
                RoleList = roleList,
                MenuList = _systemModule.GetAll(x => x.UserGroupId == id && x.ParentsId == null).Select(
                    x => new MenuViewModel
                    {
                        MenuName = x.ModuleName,
                        MenuId = x.Id,
                        IsField = x.IsField,
                        RolesList = roleList.Select(z => new RolesViewModel()
                        {
                            Id = z.Id,
                            RoleName = z.RoleName,
                            Functionality = x.AccessModuleFunctionalities.Where(o => o.IsActive).Select(o => new FunctionalityAccessViewModel
                            {
                                Id = o.Id,
                                FunctionalityId = o.FunctionalityId,
                                ModuleId = o.ModuleId,
                                Name = o.Functionality.Name
                            }).ToList()
                        }).ToList(),
                        RoleFunctionalityAccess = x.RolesModuleAccess.Select(z => new RoleModuleAccessDto
                        {
                            RoleId = z.RoleId,
                            Functionality = new FunctionalityAccessViewModel
                            {
                                Id = z.Id,
                                FunctionalityId = z.FunctionalityId ?? 0,
                                RoleId = z.RoleId,
                                ModuleId = z.ModuleId
                            }
                        }).ToList(),
                        Childs = x.Children.Select(y => new SubMenuViewModel
                        {
                            Id = y.Id,
                            ModuleName = y.MenuDisplayText,
                            IsField = y.IsField,
                            RolesList = roleList.Select(z => new RolesViewModel()
                            {
                                Id = z.Id,
                                RoleName = z.RoleName,
                                Functionality = y.AccessModuleFunctionalities.Where(o => o.IsActive).Select(o => new FunctionalityAccessViewModel
                                {
                                    Id = o.Id,
                                    FunctionalityId = o.FunctionalityId,
                                    ModuleId = o.ModuleId,
                                    Name = o.Functionality.Name
                                }).ToList()
                            }).ToList(),
                            RoleIds = y.RolesModuleAccess.Select(z => z.RoleId).ToList(),
                            RolesModuleId = y.RolesModuleAccess.Select(z => z.ModuleId).ToList(),
                            FunctionalityId = y.RolesModuleAccess.Select(z => z.FunctionalityId).ToList(),
                            RoleFunctionalityAccess = y.RolesModuleAccess.Select(z => new RoleModuleAccessDto
                            {
                                RoleId = z.RoleId,
                                Functionality = new FunctionalityAccessViewModel
                                {
                                    Id = z.Id,
                                    FunctionalityId = z.FunctionalityId ?? 0,
                                    RoleId = z.RoleId,
                                    ModuleId = z.ModuleId
                                }
                            }).ToList(),
                            Functionality = y.AccessModuleFunctionalities.Where(o => o.IsActive).Select(z => new FunctionalityAccessViewModel
                            {
                                Id = z.Id,
                                FunctionalityId = z.FunctionalityId,
                                ModuleId = z.ModuleId,
                                Name = z.Functionality.Name
                            }).ToList()
                        }).ToList(),
                        Functionality = x.AccessModuleFunctionalities.Where(o => o.IsActive).Select(z => new FunctionalityAccessViewModel
                        {
                            Id = z.Id,
                            FunctionalityId = z.FunctionalityId,
                            ModuleId = z.ModuleId,
                            Name = z.Functionality.Name
                        }).ToList(),
                        RoleIds = x.RolesModuleAccess.Select(z => z.RoleId).ToList()
                    }).ToList()
            };
            return View(@"Components/_RoleWishPermission", allMenu);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleWishPermission(AllMenu objModel)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (objModel?.MenuList != null && objModel.MenuList.Any())
                    {
                        foreach (var menu in objModel.MenuList)
                        {
                            if (menu.Childs != null && menu.Childs.Any())
                            {
                                foreach (var childMenu in menu.Childs)
                                {
                                    var roleModuleAccess = _rolesModuleAccess.GetAll(x => x.ModuleId == childMenu.Id).AsQueryable();
                                    var roleModuleAccessId = roleModuleAccess.Select(x => x.RoleId).ToList();

                                    if (childMenu.RoleIds == null || childMenu.RoleIds.Count <= 0)
                                    {
                                        var deletedId = roleModuleAccessId.Where(x => objModel.RoleList.Select(y => y.Id).Contains(x)).ToList();
                                        DeleteRoleModulePermissions(deletedId, null, roleModuleAccess, childMenu.Id);
                                    }
                                    else
                                    {
                                        // Delete Sub Menu Permission
                                        var deletedId = roleModuleAccessId.Except(childMenu.RoleIds).ToList();
                                        var deleted = deletedId.Intersect(objModel.RoleList.Select(x => x.Id)).ToList();
                                        DeleteRoleModulePermissions(deleted, null, roleModuleAccess, childMenu.Id);

                                        // Add Sub Menu Permission
                                        var addedRoleModuleId = childMenu.RoleIds.Except(roleModuleAccessId).ToList();
                                        await AddRoleModulePermissions(addedRoleModuleId, null, childMenu.Id);


                                        // Find Sub Menu Functionality Permission
                                        var functionalityExist = _rolesModuleAccess.GetAll(x => x.ModuleId == childMenu.Id && x.FunctionalityId != null && childMenu.RoleIds.Contains(x.RoleId))
                                            .AsQueryable();

                                        //Delete Sub Menu Functionality Permission
                                        DeleteRoleModuleFunctionalityPermissions(functionalityExist, childMenu.Id);

                                        //Get Sub Menu Functionality Permission
                                        var functionalityRoleList = childMenu.RolesList.Where(x => childMenu.RoleIds.Contains(x.Id))
                                            .Select(x => x.Functionality.Where(z => z.IsAccess).ToList())
                                            .ToList();

                                        foreach (var role in functionalityRoleList)
                                        {
                                            foreach (var functionality in role)
                                            {
                                                //Add Sub Menu Functionality Permission
                                                await AddRoleModuleFunctionalityPermissions(functionality.RoleId, functionality.FunctionalityId, childMenu.Id);
                                            }
                                        }
                                    }
                                }
                            }
                            if (menu.RoleIds == null || menu.RoleIds.Count <= 0)
                            {
                                var roleModuleAccess = _rolesModuleAccess.GetAll(x => x.ModuleId == menu.MenuId).AsQueryable();
                                var roleModuleAccessId = roleModuleAccess.Select(x => x.RoleId).ToList();

                                var deletedId = roleModuleAccessId.Where(x => objModel.RoleList.Select(y => y.Id).Contains(x)).ToList();
                                DeleteRoleModulePermissions(deletedId, null, roleModuleAccess, menu.MenuId);
                            }
                            else
                            {
                                var roleModuleAccess = _rolesModuleAccess.GetAll(x => x.ModuleId == menu.MenuId).AsQueryable();
                                var roleModuleAccessId = roleModuleAccess.Select(x => x.RoleId).ToList();

                                if (menu.RoleIds == null || menu.RoleIds.Count <= 0) continue;
                                var deletedId = roleModuleAccessId.Except(menu.RoleIds).ToList();
                                DeleteRoleModulePermissions(deletedId, null, roleModuleAccess, menu.MenuId);
                                var addedRoleModuleId = menu.RoleIds.Except(roleModuleAccessId).ToList();
                                await AddRoleModulePermissions(addedRoleModuleId, null, menu.MenuId);

                                // Find Sub Menu Functionality Permission
                                var functionalityExist = _rolesModuleAccess.GetAll(x => x.ModuleId == menu.MenuId && x.FunctionalityId != null && menu.RoleIds.Contains(x.RoleId))
                                    .AsQueryable();

                                //Delete Sub Menu Functionality Permission
                                DeleteRoleModuleFunctionalityPermissions(functionalityExist, menu.MenuId);

                                //Get Sub Menu Functionality Permission
                                var functionalityRoleList = menu.RolesList.Where(x => menu.RoleIds.Contains(x.Id))
                                    .Select(x => x.Functionality?.Where(z => z.IsAccess).ToList())
                                    .ToList();

                                foreach (var role in functionalityRoleList)
                                {
                                    if (role == null) continue;
                                    foreach (var functionality in role)
                                    {
                                        //Add Sub Menu Functionality Permission
                                        await AddRoleModuleFunctionalityPermissions(functionality.RoleId, functionality.FunctionalityId, menu.MenuId);
                                    }
                                }
                            }
                        }
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Access permission saved successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RoleWishPermission");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Developer Modules

        public IActionResult Functionality()
        {
            return View();
        }

        public IActionResult GetFunctionalityList(JQueryDataTableParamModel param)
        {
            try
            {
                var allList = _functionality.GetAll().Select(x => new FunctionalityDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                }).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    allList = allList.Where(x => x.Name.NullToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                }

                allList = param.sSortDir_0 == "asc" ? allList.OrderBy(x => x.Name).ToList() : allList.OrderByDescending(x => x.Name).ToList();

                var display = allList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var total = allList.Count;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = display
                });

            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetFunctionalityList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditFunctionality(int id)
        {
            if (id == 0) return View(@"Components/_AddEditFunctionality", new FunctionalityDto { Id = id });
            var result = Mapper.Map<FunctionalityDto>(_functionality.GetSingle(x => x.Id == id));
            return View(@"Components/_AddEditFunctionality", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditFunctionality(FunctionalityDto model)
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

                    if (model.Id == 0)
                    {
                        var functionality = Mapper.Map<FunctionalityDto, Functionality>(model);
                        functionality.IsActive = true;
                        await _functionality.InsertAsync(functionality, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Functionality inserted successfully.", functionality.Id);
                    }
                    var result = _functionality.GetSingle(x => x.Id == model.Id);
                    result.Name = model.Name;
                    result.IsActive = model.IsActive;
                    await _functionality.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Functionality updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditFunctionality");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveFunctionality(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _functionality.GetSingle(x => x.Id == id);
                    _functionality.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Functionality deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveFunctionality");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageFunctionalityStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _functionality.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _functionality.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Functionality {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageFunctionalityStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult SystemModule()
        {
            ViewBag.UserGroup = EnumHelpers.EnumToList<GlobalEnums.UserGroup>().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Value.ToString()
            }).OrderBy(x => x.Text).ToList();
            return View();
        }

        public async Task<IActionResult> GetSystemModuleList(int id, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@UserGroupId", SqlDbType.Int) { Value = id });
                var allList = await _systemModule.GetSystemModuleList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetSystemModuleList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditSystemModule(int userGroup, int id)
        {
            ViewBag.ParentModuleList = _systemModule.GetAll(x => x.ParentsId == null && x.UserGroupId == userGroup).Select(x => new SelectListItem
            {
                Text = x.ModuleName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text).ToList();

            //ViewBag.UserGroupList = EnumHelpers.EnumToList<GlobalEnums.UserGroup>().Select(x => new SelectListItem
            //{
            //    Text = x.Name,
            //    Value = x.Value.ToString()
            //}).ToList();

            if (id == 0) return View(@"Components/_AddEditSystemModule", new SystemModuleDto { Id = id, UserGroupId = userGroup });
            var result = Mapper.Map<SystemModuleDto>(_systemModule.GetSingle(x => x.Id == id));
            return View(@"Components/_AddEditSystemModule", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditSystemModule(SystemModuleDto model)
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

                    if (model.Id == 0)
                    {
                        var systemModule = Mapper.Map<SystemModuleDto, SystemModule>(model);
                        await _systemModule.InsertAsync(systemModule, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "System module inserted successfully.", systemModule.Id);
                    }
                    var result = _systemModule.GetSingle(x => x.Id == model.Id);
                    result.ModuleName = model.ModuleName;
                    result.ParentsId = model.ParentsId;
                    result.MenuDisplayText = model.MenuDisplayText;
                    result.Controller = model.Controller;
                    result.Action = model.Action;
                    result.IsField = model.IsField;
                    result.UserGroupId = model.UserGroupId;
                    await _systemModule.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "System module updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditSystemModule");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveSystemModule(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _systemModule.GetSingle(x => x.Id == id);
                    _systemModule.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"System module deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveSystemModule");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult AccessModuleFunctionality(int id)
        {
            var result = _systemModule.GetById(id);
            var model = new SystemModuleDto { Id = result.Id, MenuDisplayText = result.MenuDisplayText };
            return View(model);
        }

        public IActionResult GetAccessModuleFunctionalityList(int id, JQueryDataTableParamModel param)
        {
            try
            {
                var allList = _accessModuleFunctionality.GetAll(x => x.ModuleId == id).Select(x => new FunctionalityDto
                {
                    Id = x.Id,
                    Name = x.Functionality.Name,
                    IsActive = x.IsActive
                }).ToList();

                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    allList = allList.Where(x => x.Name.NullToString().ToLower().Contains(param.sSearch.ToLower())).ToList();
                }

                allList = param.sSortDir_0 == "asc" ? allList.OrderBy(x => x.Name).ToList() : allList.OrderByDescending(x => x.Name).ToList();

                var display = allList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var total = allList.Count;

                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = display
                });

            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetFunctionalityList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditAccessFunctionality(int moduleId, int id)
        {
            ViewBag.FunctionalityList = _functionality.GetAll(x => x.IsActive)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
                .ToList();

            if (id == 0) return View(@"Components/_AddEditAccessFunctionality", new AccessModuleFunctionalityDto { Id = id, ModuleId = moduleId });
            var result = Mapper.Map<AccessModuleFunctionalityDto>(_accessModuleFunctionality.GetSingle(x => x.Id == id && x.ModuleId == moduleId));
            return View(@"Components/_AddEditAccessFunctionality", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditAccessFunctionality(AccessModuleFunctionalityDto model)
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

                    if (model.Id == 0)
                    {
                        var systemModule = new AccessModuleFunctionality
                        {
                            ModuleId = model.ModuleId,
                            FunctionalityId = model.FunctionalityId,
                            IsActive = true
                        };
                        await _accessModuleFunctionality.InsertAsync(systemModule, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Access functionality inserted successfully.", systemModule.Id);
                    }
                    var result = _accessModuleFunctionality.GetSingle(x => x.Id == model.Id);
                    result.FunctionalityId = model.FunctionalityId;
                    await _accessModuleFunctionality.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Access functionality updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditAccessFunctionality");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageAccessFunctionalityStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _accessModuleFunctionality.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _accessModuleFunctionality.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Access Functionality {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageDosageFormStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveAccessFunctionality(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _accessModuleFunctionality.GetSingle(x => x.Id == id);
                    _accessModuleFunctionality.Delete(result);
                    _dosageForm.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Access Functionality deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveAccessFunctionality");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region ContactDetail
        public IActionResult ContactDetail()
        {
            var model = Mapper.Map<ContactDetailView>(_contactUs.GetAll(x => x.IsActive).FirstOrDefault());
            return View(model ?? new ContactDetailView() { Id = 0 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveContactDetail(ContactDetailView model)
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
                    if (model.Id == 0)
                    {
                        var contactDetails = Mapper.Map<ContactDetailView, ContactDetails>(model);
                        contactDetails.IsActive = true;
                        await _contactUs.InsertAsync(contactDetails, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Contact detail saved successfully.");
                    }
                    var editcontactUs = _contactUs.GetSingle(x => x.Id == model.Id);
                    editcontactUs.Email = model.Email;
                    editcontactUs.Address = model.Address;
                    editcontactUs.City = model.City;
                    editcontactUs.State = model.State;
                    editcontactUs.ZipCode = model.ZipCode;
                    editcontactUs.Telephone = model.Telephone;
                    editcontactUs.Fax = model.Fax;
                    await _contactUs.UpdateAsync(editcontactUs, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Contact detail updated successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/SaveContactDetail");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Contact Request
        public IActionResult ContactRequest()
        {
            return View();
        }

        public async Task<IActionResult> GetContactRequestList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _contactRequest.GetContactRequestListAsync(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetContactRequestList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }
        #endregion

        #region Document

        public IActionResult Document()
        {
            return View();
        }

        public async Task<IActionResult> GetDocumentList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _document.GetDocumentList(parameters.ToArray());
                allList.ForEach(x =>
                {
                    x.DocumentFile = $@"{FilePathList.Document}\{x.DocumentFile}";
                    x.TypeName = ((GlobalEnums.DocumentType)x.DocumentType).GetDescription();
                });
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
                ErrorLog.AddErrorLog(ex, "GetDocumentList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditDocument(long id)
        {
            ViewBag.DocumentTypeMaster = EnumHelpers.EnumToList<GlobalEnums.DocumentType>().Select(x =>
                new SelectListItem() { Text = ((GlobalEnums.DocumentType)x.Value).GetDescription(), Value = x.Value.ToString() }).OrderBy(x => x.Text).ToList();
            if (id == 0) return View(@"Components/_AddEditDocument", new DocumentView() { Id = id });
            var result = _document.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditDocument", new DocumentView { Id = result.Id, DocumentName = result.DocumentName, DocumentType = result.DocumentType });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditDocument(DocumentView model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string newDocumentFile = string.Empty;
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    if (model.Document != null)
                    {
                        newDocumentFile = CommonMethod.GetFileName(model.Document.FileName);
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Document, newDocumentFile, model.Document);
                    }
                    var isExistName = _document.GetCount(x => x.DocumentName.ToLower().Equals(model.DocumentName));
                    if (model.Id == 0)
                    {
                        var document = Mapper.Map<DocumentView, DocumentMaster>(model);
                        var startsWithWhiteSpace = char.IsWhiteSpace(model.DocumentName, 0);
                        if (startsWithWhiteSpace || isExistName > 0)
                        {
                            var name = model.DocumentName.Replace(" ", String.Empty);
                            document.DocumentName = name;
                            var isExistAfterSpace = _document.GetCount(x => x.DocumentName.ToLower().Equals(name));

                            if (isExistAfterSpace > 0 || isExistName > 0)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, "Document Name already exists");
                            }
                        }
                        else
                        {
                            document.DocumentName = model.DocumentName;
                        }
                        document.DocumentFile = newDocumentFile;
                        document.IsActive = true;
                        await _document.InsertAsync(document, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Document inserted successfully.", document.Id);
                    }
                    else
                    {
                        var isExist = _document.GetCount(x => x.DocumentName.ToLower().Equals(model.DocumentName.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Document Name already exists");
                        }
                        var result = _document.GetSingle(x => x.Id == model.Id);
                        var oldDocumentFile = result.DocumentFile;
                        result.DocumentType = model.DocumentType;
                        result.DocumentName = model.DocumentName;
                        result.DocumentFile = !string.IsNullOrEmpty(newDocumentFile) ? newDocumentFile : oldDocumentFile;
                        await _document.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        if (!string.IsNullOrEmpty(newDocumentFile) && !string.IsNullOrEmpty(oldDocumentFile))
                        {
                            CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, oldDocumentFile), true);
                        }
                        return JsonResponse.GenerateJsonResult(1, "Document updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    if (!string.IsNullOrEmpty(newDocumentFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, newDocumentFile), true);
                    }
                    ErrorLog.AddErrorLog(ex, "Post/AddEditDocument");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageDocumentStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _document.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _document.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Document {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageDocumentStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveDocument(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _document.GetSingle(x => x.Id == id);
                    _document.Delete(result);
                    txscope.Complete();
                    CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Document, result.DocumentFile), true);
                    return JsonResponse.GenerateJsonResult(1, @"Document deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveDocument");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region DosageForm
        public IActionResult DosageForm()
        {
            return View();
        }

        public async Task<IActionResult> GetDosageFormList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _dosageForm.GetDosageFormList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetDosageFormList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditDosageForm(long id)
        {
            if (id == 0) return View(@"Components/_AddEditDosageForm", new DosageFormView { Id = id });
            var result = _dosageForm.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditDosageForm", new DosageFormView { Id = result.Id, DosageForm = result.DosageForm });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditDosageForm(DosageFormView model)
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

                    if (model.Id == 0)
                    {
                        var isExist = _dosageForm.GetCount(x => x.DosageForm.ToLower().Equals(model.DosageForm.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Dosage Form already exists.");
                        }
                        var dosageFormMaster = new DosageFormMaster { DosageForm = model.DosageForm, IsActive = true };

                        await _dosageForm.InsertAsync(dosageFormMaster, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Dosage Form inserted successfully.", dosageFormMaster.Id);
                    }
                    else
                    {
                        var isExist = _dosageForm.GetCount(x => x.DosageForm.ToLower().Equals(model.DosageForm.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Dosage Form already exists.");
                        }
                        var result = _dosageForm.GetSingle(x => x.Id == model.Id);
                        result.DosageForm = model.DosageForm;
                        await _dosageForm.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Dosage Form updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditDosageForm");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageDosageFormStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _dosageForm.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _dosageForm.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Dosage Form {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageDosageFormStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveDosageForm(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _dosageForm.GetSingle(x => x.Id == id);
                    _dosageForm.Delete(result);
                    _dosageForm.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Dosage Form deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveDosageForm");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Email Template

        public async Task<IActionResult> GetAdvertiseTemplateList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _advertiseTemplate.GetAdvertiseTemplateList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetAdvertiseTemplateList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public ActionResult EmailTemplate(long id = 0)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageTemplateStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _advertiseTemplate.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _advertiseTemplate.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Advertise Email Template {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageTemplateStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public IActionResult EmailTemplateAddEdit(long id)
        {
            if (id == 0) return View("EmailTemplateAddEdit", new AdvertiseTemplateDto { Id = id });
            var result = Mapper.Map<AdvertiseTemplateDto>(_advertiseTemplate.GetSingle(x => x.Id == id));
            return View("EmailTemplateAddEdit", result);
        }

        public IActionResult ViewEmailTemplate(long id)
        {
            var result = Mapper.Map<AdvertiseTemplateDto>(_advertiseTemplate.GetSingle(x => x.Id == id));
            return View(@"Components/_ViewEmailTemplate", result);
        }

        [HttpPost]
        public async Task<IActionResult> EmailTemplateAddEdit(AdvertiseTemplateDto model)
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

                    if (model.Id == 0)
                    {
                        var template = Mapper.Map<AdvertiseTemplateDto, AdvertiseEmailTemplate>(model);
                        //var name = StripTagsRegex(model.TemplateName);
                        //template.TemplateName = name;
                        template.TemplateName = model.TemplateName;
                        template.IsActive = true;

                        await _advertiseTemplate.InsertAsync(template, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Advertise Email Template inserted successfully.", template.Id);
                    }
                    var result = _advertiseTemplate.GetSingle(x => x.Id == model.Id);
                    result.TemplateName = model.TemplateName;
                    result.Template = model.Template;
                    await _advertiseTemplate.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Advertise Email Template updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/EmailTemplateAddEdit");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveAdvertiseTemplate(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _advertiseTemplate.GetSingle(x => x.Id == id);
                    _advertiseTemplate.Delete(result);
                    _advertiseTemplate.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Advertise Email Template deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveAdvertiseTemplate");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region FaQs
        public IActionResult Faqs()
        {
            return View();
        }

        public async Task<IActionResult> GetFaqsList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _faQs.GetFaQsListAsync(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetFaqsList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditFaqs(long id)
        {
            if (id == 0) return View(@"Components/_AddEditFaqs", new FaQsView() { Id = id });
            var result = _faQs.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditFaqs", new FaQsView { Id = result.Id, Answer = result.Answer, Question = result.Question });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditFaqs(FaQsView model)
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

                    if (model.Id == 0)
                    {
                        var isExist = _faQs.GetCount(x => x.Question.ToLower().Equals(model.Question.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Question already exists.");
                        }
                        var faQs = new FAQs { Question = model.Question, Answer = model.Answer, IsActive = true };

                        await _faQs.InsertAsync(faQs, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "FAQs inserted successfully.", faQs.Id);
                    }
                    else
                    {
                        var isExist = _faQs.GetCount(x => x.Question.ToLower().Equals(model.Question.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Question already exists.");
                        }
                        var result = _faQs.GetSingle(x => x.Id == model.Id);
                        result.Question = model.Question;
                        result.Answer = model.Answer;
                        await _faQs.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "FAQs updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditFaqs");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageFaQStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _faQs.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _faQs.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"FAQs {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageFaQStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveFaQs(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _faQs.GetSingle(x => x.Id == id);
                    _faQs.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"FAQs deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveFaQs");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Manufacturer
        public IActionResult Manufacturer()
        {
            return View();
        }

        public async Task<IActionResult> GetManufacturerList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _manufacturer.GetManufacturerList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetManufacturerList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditManufacturer(long id)
        {
            if (id == 0) return View(@"Components/_AddEditManufacturer", new ManufacturerView { Id = id });
            var result = _manufacturer.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditManufacturer", new ManufacturerView { Id = result.Id, ManufacturerName = result.ManufacturerName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditManufacturer(ManufacturerView model)
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

                    if (model.Id == 0)
                    {
                        var isExist = _manufacturer.GetCount(x => x.ManufacturerName.ToLower().Equals(model.ManufacturerName.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Manufacturer already exists.");
                        }
                        var manufacturerMaster = new ManufacturerMaster { ManufacturerName = model.ManufacturerName, IsActive = true };

                        await _manufacturer.InsertAsync(manufacturerMaster, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Manufacturer inserted successfully.", manufacturerMaster.Id);
                    }
                    else
                    {
                        var isExist = _manufacturer.GetCount(x => x.ManufacturerName.ToLower().Equals(model.ManufacturerName.ToLower())
                                                            && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Manufacturer already exists.");
                        }
                        var result = _manufacturer.GetSingle(x => x.Id == model.Id);
                        result.ManufacturerName = model.ManufacturerName;
                        await _manufacturer.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Manufacturer updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditManufacturer");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageManufacturerStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _manufacturer.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _manufacturer.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Manufacturer {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageManufacturerStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveManufacturer(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _manufacturer.GetSingle(x => x.Id == id);
                    _manufacturer.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Manufacturer deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveManufacturer");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Measurement

        public IActionResult Measurement()
        {
            ViewBag.MeasurementList = EnumHelpers.EnumToList<GlobalEnums.MeasurementType>().Select(x => new SelectListItem
            {
                Text = ((GlobalEnums.MeasurementType)x.Value).GetDescription(),
                Value = x.Value.ToString()
            }).OrderBy(x => x.Text);
            return View();
        }

        public async Task<IActionResult> GetMeasurementList(int measurementTypeId, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@measurementTypeId", SqlDbType.Int) { Value = measurementTypeId });
                var allList = await _measurement.GetMeasurementListAsync(parameters.ToArray());
                //allList.ForEach(x =>
                //{
                //    x.MeasurementTypeName = ((GlobalEnums.MeasurementType)x.MeasurementType).GetDescription();
                //});
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
                ErrorLog.AddErrorLog(ex, "GetMeasurementList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditMeasurement(long id)
        {
            ViewBag.MeasurementType = EnumHelpers.EnumToList<GlobalEnums.MeasurementType>().Select(x => new SelectListItem
            {
                Text = ((GlobalEnums.MeasurementType)x.Value).GetDescription(),
                Value = x.Value.ToString()
            }).OrderBy(x => x.Text).ToList();

            if (id == 0) return View(@"Components/_AddEditMeasurement", new MeasurementView() { Id = id });
            var result = _measurement.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditMeasurement", new MeasurementView { Id = result.Id, MeasurementType = result.MeasurementType, MeasurementUnit = result.MeasurementUnit });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditMeasurement(MeasurementView model)
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

                    if (model.Id == 0)
                    {
                        var isExist = _measurement.GetCount(x => x.MeasurementType.Equals(model.MeasurementType) && x.MeasurementUnit.ToLower().Equals(model.MeasurementUnit.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Mesurement Type already exist.");
                        }
                        var measurement = new Measurement { MeasurementType = model.MeasurementType, MeasurementUnit = model.MeasurementUnit, IsActive = true };

                        await _measurement.InsertAsync(measurement, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Measurement inserted successfully.", measurement.Id);
                    }
                    else
                    {
                        var isExist = _measurement.GetCount(x => x.MeasurementType.Equals(model.MeasurementType) && x.MeasurementUnit.ToLower().Equals(model.MeasurementUnit.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Measurement already exist.");
                        }
                        var result = _measurement.GetSingle(x => x.Id == model.Id);
                        result.MeasurementType = model.MeasurementType;
                        result.MeasurementUnit = model.MeasurementUnit;
                        await _measurement.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Measurement updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditMeasurement");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMeasurementStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _measurement.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _measurement.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Measurement {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageMeasurementStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveMeasurement(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _measurement.GetSingle(x => x.Id == id);
                    _measurement.Delete(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Measurement deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveMeasurement");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region MedicineCategory
        public IActionResult MedicineCategory()
        {
            return View();
        }

        public async Task<IActionResult> GetMedicineCategoryList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                var allList = await _medicineCategory.GetMecineCategoryList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetMedicineCategoryList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditMedicineCategory(long id)
        {
            if (id == 0) return View(@"Components/_AddEditMedicineCategory", new MedicineCategoryView { Id = id });
            var result = _medicineCategory.GetSingle(x => x.Id == id);
            return View(@"Components/_AddEditMedicineCategory", new MedicineCategoryView { Id = result.Id, MedicineCategory = result.MedicineCategory });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditMedicineCategory(MedicineCategoryView model)
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

                    if (model.Id == 0)
                    {
                        var isExist = _medicineCategory.GetCount(x => x.MedicineCategory.ToLower().Equals(model.MedicineCategory.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Medicine Category already exist.");
                        }
                        var medicineCategoryMaster = new MedicineCategoryMaster { MedicineCategory = model.MedicineCategory, IsActive = true };
                        await _medicineCategory.InsertAsync(medicineCategoryMaster, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Medicine Category inserted successfully.", medicineCategoryMaster.Id);
                    }
                    else
                    {
                        var isExist = _medicineCategory.GetCount(x => x.MedicineCategory.ToLower().Equals(model.MedicineCategory.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Medicine Category already exist.");
                        }
                        var result = _medicineCategory.GetSingle(x => x.Id == model.Id);
                        result.MedicineCategory = model.MedicineCategory;
                        await _medicineCategory.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Medicine Category updated successfully.", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditMedicineCategory");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMedicineCategoryStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _medicineCategory.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _medicineCategory.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Medicine Category {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageMedicineCategoryStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveMedicineCategory(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _medicineCategory.GetSingle(x => x.Id == id);
                    _medicineCategory.Delete(result);
                    _medicineCategory.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Medicine Category deleted successfully");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveMedicineCategory");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region NewDistributorRequest
        public IActionResult NewDistributorRequest()
        {
            return View();
        }

        public async Task<IActionResult> GetNewDistributorRequestList(JQueryDataTableParamModel param, bool status)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@Status", SqlDbType.TinyInt) { Value = status });
                var allList = await _newDistributor.GetNewDistributorRequestList(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetNewDistributorRequestList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [Route("CreateOrViewDistributorAccount/{id}/{isView}")]
        public IActionResult CreateOrViewDistributorAccount(long id, bool isView)
        {
            ViewBag.isView = isView;
            if (id == 0) return View(@"Components/_CreateOrViewDistributorAccount", new NewDistributorRequestDto { Id = id });
            var model = Mapper.Map<NewDistributorRequestDto>(_newDistributor.GetSingle(x => x.Id == id));
            return View(@"Components/_CreateOrViewDistributorAccount", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNewDistributor(NewDistributorRequestDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _newDistributor.GetSingle(x => x.Id == model.Id);
                    var user = await _userManager.FindByEmailAsync(result.Email);
                    if (user != null)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, "Distributor already created.");
                    }
                    var distributorUser = new ApplicationUser
                    {
                        Email = result.Email,
                        UserName = result.Email,
                        Mobile = result.Mobile,
                        PhoneNumber = result.Phone,
                        IsActive = true
                    };
                    string password = CommonMethod.CreateRandomPassword(8);
                    await _userManager.CreateAsync(distributorUser, password);
                    await _userManager.AddToRoleAsync(distributorUser, UserRoles.DistributorPrimaryAdmin);
                    var newdistributor = new DbDistributor
                    {
                        UserId = distributorUser.Id,
                        CompanyName = result.CompanyName,
                        Email = result.Email,
                        Mobile = result.Mobile,
                        Phone = result.Phone,
                        Address = result.Address,
                        ZipCode = result.ZipCode,
                        StateId = result.StateId,
                        City = result.City,
                        ContactName = result.ContactName,
                        ContactEmail = result.ContactEmail,
                        ContactMobile = result.ContactMobile,
                        ContactAddress = result.ContactAddress,
                        ContactCity = result.ContactCity,
                        ContactStateId = result.ContactStateId,
                        ContactZipCode = result.ContactZipCode,
                        IsActive = true
                    };
                    await _distributer.InsertAsync(newdistributor, Accessor, User.GetUserId());
                    distributorUser.DistributorId = newdistributor.Id;

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
                    await _userManager.UpdateAsync(distributorUser);
                    
                    if (newdistributor.Id != 0)
                    {
                        await _newDistributor.DeleteAsync(result, Accessor, User.GetUserId());
                        _newDistributor.Save();
                    }

                    //For providing default subscription(PENDING)
                    //var subscriptionType = _subscriptionType.GetSingle(x => x.Id == (long)GlobalEnums.SubscriptionTypes.Silver);
                    //DateTime startdate = DateTime.UtcNow;

                    //var disSubscription = new DistributorSubscription
                    //{
                    //    DistributorId = newdistributor.Id,
                    //    StartDate = startdate,
                    //    EndDate = startdate.AddMonths(6),
                    //    SubscriptionTypeId = subscriptionType.Id,
                    //    ChargedMonthly = subscriptionType.ChargedMonthly,
                    //    SubscriptionCharge = subscriptionType.SubscriptionCharge,
                    //    IsActive = true,
                    //    IsPayment = false
                    //};
                    //await _distributorSubscription.InsertAsync(disSubscription, Accessor, User.GetUserId());

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(distributorUser);
                    var callbackUrl = Url.EmailConfirmationLink(distributorUser.Id, code, Request.Scheme);

                    var emailTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.CreateDistributor, GetPhysicalUrl());
                    emailTemplate = emailTemplate.Replace("{UserName}", distributorUser.FullName);
                    emailTemplate = emailTemplate.Replace("{Password}", password);
                    emailTemplate = emailTemplate.Replace("{action_url}", callbackUrl);
                    await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                    {
                        ToAddress = newdistributor.Email,
                        Subject = "Distributor created successfully",
                        BodyText = emailTemplate
                    });

                    //#region Congratulation email
                    //var congratulationTemplate = CommonMethod.ReadEmailTemplate(ErrorLog, HostingEnvironment.WebRootPath, EmailTemplateFileList.Congratulation, GetPhysicalUrl());
                    //congratulationTemplate = congratulationTemplate.Replace("{acounttype}", "Distributer");
                    //congratulationTemplate = congratulationTemplate.Replace("{email}", distributorUser.Email);
                    //await _emailService.SendEmailAsyncByGmail(new SendEmailModel
                    //{
                    //    ToAddress = distributorUser.Email,
                    //    Subject = "Congratulation",
                    //    BodyText = congratulationTemplate
                    //});
                    //#endregion
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Distributor created successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/CreateNewDistributor");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageDistributorStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var newdistributor = _newDistributor.GetSingle(x => x.Id == id);
                    if (!newdistributor.IsActive)
                    {
                        var user = await _userManager.FindByEmailAsync(newdistributor.Email);
                        if (user != null)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Distributor already created.");
                        }
                    }
                    newdistributor.IsActive = !newdistributor.IsActive;

                    await _newDistributor.UpdateAsync(newdistributor, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Distributor {(newdistributor.IsActive ? "approved" : "rejected")} successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-ManageDistributorStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistributorRequest(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var newdistributor = _newDistributor.GetSingle(x => x.Id == id);
                    if (newdistributor.Id != 0)
                    {
                        await _newDistributor.DeleteAsync(newdistributor, Accessor, User.GetUserId());
                        _newDistributor.Save();
                    }
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Distributor request deleted successfully.");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-DeleteDistributorRequest");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region Testimonials
        public IActionResult Testimonials()
        {
            return View();
        }

        public async Task<IActionResult> GetTestimonialsList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _testimonials.GetTestimonialsListAsync(parameters.ToArray());
                allList.ForEach(x => { x.Image = $@"{FilePathList.Testimonial}\{x.Image}"; });

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
                ErrorLog.AddErrorLog(ex, "GetTestimonialsList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditTestimonials(long id)
        {
            if (id == 0) return View(@"Components/_AddEditTestimonials", new TestimonialDto() { Id = id });
            var result = Mapper.Map<TestimonialDto>(_testimonials.GetSingle(x => x.Id == id));
            result.Image = result.Image == null ? "~/rxfairbackend/images/pop_user.png" : $@"\{FilePathList.Testimonial}\{result.Image}";
            return View(@"Components/_AddEditTestimonials", result);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditTestimonials(TestimonialDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                string newTestimonialFile = string.Empty;
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    if (model.TestimonialFile != null)
                    {
                        newTestimonialFile = CommonMethod.GetFileName(model.TestimonialFile.FileName);
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.Testimonial, newTestimonialFile, model.TestimonialFile);
                    }
                    if (model.Id == 0)
                    {
                        var testimonials = Mapper.Map<TestimonialDto, Testimonials>(model);
                        testimonials.Image = newTestimonialFile;
                        testimonials.IsActive = true;
                        await _testimonials.InsertAsync(testimonials, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Testimonial inserted successfully.", testimonials.Id);
                    }
                    var result = _testimonials.GetSingle(x => x.Id == model.Id);
                    var oldTestimonialFile = result.Image;
                    result.Name = model.Name;
                    result.Feedback = model.Feedback;
                    result.Image = !string.IsNullOrEmpty(newTestimonialFile) ? newTestimonialFile : oldTestimonialFile;
                    await _testimonials.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    if (!string.IsNullOrEmpty(newTestimonialFile) && !string.IsNullOrEmpty(oldTestimonialFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Testimonial, oldTestimonialFile), true);
                    }
                    return JsonResponse.GenerateJsonResult(1, "Testimonial updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    if (!string.IsNullOrEmpty(newTestimonialFile))
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Testimonial, newTestimonialFile), true);
                    }
                    ErrorLog.AddErrorLog(ex, "Post/AddEditTestimonials");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageTestimonialstatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _testimonials.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _testimonials.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Testimonial {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageTestimonialstatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveTestimonials(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _testimonials.GetSingle(x => x.Id == id);
                    _testimonials.Delete(result);
                    txscope.Complete();
                    CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.Testimonial, result.Image), true);
                    return JsonResponse.GenerateJsonResult(1, @"Testimonial deleted successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveTestimonials");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region TermsConditions
        public IActionResult TermsConditions()
        {
            var result = _termsAndCondition.GetAll(x => x.IsActive).FirstOrDefault();
            return View(new TermsAndConditionDto()
            {
                Id = result?.Id ?? 0,
                TermsCondition = result?.TermsCondition
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> TermsConditions(TermsAndConditionDto model)
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

                    if (model.Id == 0)
                    {
                        var termsCondition = new TermsAndCondition()
                        {
                            TermsCondition = model.TermsCondition,
                            IsActive = true
                        };
                        await _termsAndCondition.InsertAsync(termsCondition, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Terms & Conditions inserted successfully.", termsCondition.Id);
                    }
                    var result = _termsAndCondition.GetSingle(x => x.Id == model.Id);
                    result.TermsCondition = model.TermsCondition;
                    await _termsAndCondition.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Terms & Conditions updated successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/TermsConditions");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #endregion

        #region Controller Common
        private async Task AddRoleModulePermissions(IReadOnlyCollection<long> roleId, IReadOnlyCollection<FunctionalityAccessViewModel> functionality, int menuId)
        {
            if (roleId == null || !roleId.Any()) return;
            var objList = roleId.Select(objAddedRoleModule => new RolesModuleAccess
            {
                ModuleId = menuId,
                RoleId = objAddedRoleModule,
                FunctionalityId = functionality?.FirstOrDefault(x => x.RoleId == objAddedRoleModule)?.FunctionalityId
            }).ToList();

            if (!objList.Any()) return;
            await _rolesModuleAccess.InsertRangeAsync(objList, Accessor, User.GetUserId());
        }

        private async Task AddRoleModuleFunctionalityPermissions(long roleId, int functionalityId, int menuId)
        {
            var objList = new RolesModuleAccess
            {
                ModuleId = menuId,
                RoleId = roleId,
                FunctionalityId = functionalityId
            };
            await _rolesModuleAccess.InsertAsync(objList, Accessor, User.GetUserId());
        }

        private void DeleteRoleModulePermissions(ICollection<long> deletedId, ICollection<int?> functionality, IQueryable<RolesModuleAccess> roleModuleAccess, int menuId)
        {
            if (deletedId == null || !deletedId.Any()) return;
            var deletedRoleModuleAccess = roleModuleAccess.Where(x => x.ModuleId == menuId && deletedId.Contains(x.RoleId))
                .ToList();
            if (!deletedRoleModuleAccess.Any()) return;
            _rolesModuleAccess.DeleteRange(deletedRoleModuleAccess);
        }

        private void DeleteRoleModuleFunctionalityPermissions(IQueryable<RolesModuleAccess> roleModuleAccess, int menuId)
        {
            if (roleModuleAccess == null || !roleModuleAccess.Any()) return;
            var deletedRoleModuleAccess = roleModuleAccess.Where(x => x.ModuleId == menuId)
                .ToList();
            if (!deletedRoleModuleAccess.Any()) return;
            _rolesModuleAccess.DeleteRange(deletedRoleModuleAccess);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult CheckUserIsExist(string email)
        {
            var isExist = _newDistributor.GetSingle(x => x.Email.Equals(email));
            var isExist1 = _user.GetSingle(x => x.UserName.Equals(email));
            if (isExist != null || isExist1 != null)
            {
                return JsonResponse.GenerateJsonResult(1, GlobalConstant.AlreadyRegisterd);
            }
            return JsonResponse.GenerateJsonResult(0, GlobalConstant.EmailNotFound);
        }

        //public static string StripTagsRegex(string source)
        //{

        //    return Regex.Replace(source, "<(.|;&\n)*?>", "");

        //}
        #endregion
    }
}