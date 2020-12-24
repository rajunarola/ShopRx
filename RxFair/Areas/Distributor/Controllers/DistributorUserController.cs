using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RxFair.Models;
using RxFair.Service.Interface;
using RxFair.Utility.Common;
using RxFair.Utility;
using System.Transactions;
using RxFair.Data.DbContext;
using System.Data.SqlClient;
using System.Data;
using RxFair.Utility.Extension;
using RxFair.Dto.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RxFair.Service.Utility;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class DistributorUserController : BaseController<DistributorUserController>
    {
        #region Fields
        private readonly EmailService _emailService;
        private readonly IDistributorService _distributor;
        private readonly IUserService _user;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region ctor
        public DistributorUserController(IOptions<EmailSettingsGmail> emailSettingsGmail, IDistributorService distributor, IUserService user, UserManager<ApplicationUser> userManager)
        {
            _emailService = new EmailService(emailSettingsGmail);
            _distributor = distributor;
            _user = user;
            _userManager = userManager;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetUserList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                parameters.Parameters.Insert(0, new SqlParameter("@DistributorId", SqlDbType.BigInt) { Value = User.GetClaimValue(UserClaims.DistributorId) });

                var allList = await _distributor.GetDistributorUserList(parameters.Parameters.ToArray());
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

        [HttpPost]
        public async Task<IActionResult> DistributorUserStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = await _userManager.FindByIdAsync(id.ToString());
                    result.IsActive = !result.IsActive;
                    await _userManager.UpdateAsync(result);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"User {(result.IsActive ? "activated" : "deactivated")} successfully.", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/DistributorUserStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region Common
        public IList<string> GetUserRoles(ApplicationUser user)
        {
            var rolesList = Task.Run(() => _userManager.GetRolesAsync(user)).Result;
            return rolesList;
        }

        public void BindDropdownList()
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserGroupId", SqlDbType.Int) { Value = 2 }
            };
            var roleList = Task.Run(async () => await _user.GetUserGroupWishRole(parameters.ToArray())).Result;
            ViewBag.RoleList = roleList.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name
            }).OrderBy(x => x.Text).ToList();

        }
        #endregion
    }
}