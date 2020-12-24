using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using RxFair.Data.DbContext;
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
    public class ManageUsersController : BaseController<MasterController>
    {
        #region Fields
        private readonly EmailService _emailService;
        private readonly IUserService _user;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<Role> _roleManager;

        #endregion

        #region Ctor
        public ManageUsersController(IOptions<EmailSettingsGmail> emailSettingsGmail, IUserService user, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _user = user;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        #region User
        [Route("ManageUsers/SystemUser")]
        [Route("ManageUsers/PharmacyUser")]
        [Route("ManageUsers/DistributorUser")]
        public IActionResult ManageUser()
        {
            var route = HttpContext.Request.Path.Value.Split("/");
            ViewBag.UserGroupName = route[2];
            ViewBag.UserGroupId = route[2] == "SystemUser" ? UserRoleGroup.Admin : route[2].Replace("User", "");
            return View();
        }

        [Route("ManageUsers/GetUserList/{userGroup}")]
        public async Task<IActionResult> GetUserList(string userGroup, JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;
                parameters.Insert(0, new SqlParameter("@UserGroup", SqlDbType.Int) { Value = CommonMethod.GetUserGroupId(userGroup) });
                var allList = await _user.GetUserListAsync(parameters.ToArray());
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
                ErrorLog.AddErrorLog(ex, "GetUserList");
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

        #region ManageIsActive
        [HttpPost]
        public async Task<IActionResult> ManageIsActive(long id)
        {
            try
            {
                var users = _user.GetSingle(x => x.Id == id);
                var role = GetUserRoles(users).FirstOrDefault();
                 users.IsActive = !users.IsActive;
                await _user.UpdateAsync(users, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"{CommonMethod.GetUserGroupName(role)} user {(users.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-ManageIsActive");
                return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
            }
        }
        #endregion

        #endregion

        #region Controller Common

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
                    roleList = new List<long> { 2, 3, 4, 5 };
                    break;
                case UserRoleGroup.Distributor:
                    roleList = new List<long> { 6, 7, 8 };
                    break;
                case UserRoleGroup.Pharmacy:
                    roleList = new List<long> { 9, 10, 11 };
                    break;
            }

            ViewBag.RoleList = _roleManager.Roles.Where(x => roleList.Contains(x.Id)).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Name
            }).OrderBy(x => x.Text);
        }
        #endregion

    }
}