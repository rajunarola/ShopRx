using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Utility;
using System.Threading.Tasks;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class DashboardController : BaseController<DashboardController>
    {
        private readonly IUserService _user;

        public DashboardController(IUserService user)
        {
            _user = user;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var distributorInfoList = await _user.GetDistributorSubInfoList();
            ViewBag.distributorInfoList = distributorInfoList;

            var model = _user.GetAdminDashboard();
            
            return View(model);
        }

    }
}

