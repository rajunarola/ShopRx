using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Pharmacy.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Pharmacy), Area("Pharmacy")]
    public class DashboardController : BaseController<DashboardController>
    {
        private readonly IUserService _user;
        private readonly IPharmacyService _pharmacy;

        public DashboardController(IUserService user, IPharmacyService pharmacy)
        {
            _user = user;
            _pharmacy = pharmacy;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@PharmacyDistributorId", SqlDbType.BigInt) { Value = User.GetClaimValue(UserClaims.PharmacyId) },
                new SqlParameter("@isPharmacy", SqlDbType.Bit) { Value = true }
            };
            var model = _user.GetPharmacyDistributorUserDashboard(parameters.ToArray());
            ViewBag.OrderDistributorList = _pharmacy.GetOrderDistributorList().Select(x => new SelectListItem { Text = x.Text, Value = x.Value }).OrderBy(x => x.Text).ToList();
            return View(model);
        }
    }
}

