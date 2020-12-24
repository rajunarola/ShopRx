using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RxFair.Dto.Enum;
using RxFair.Service.Interface;
using RxFair.Utility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Distributor.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Distributor), Area("Distributor")]
    public class DashboardController : BaseController<DashboardController>
    {
        private readonly IUserService _user;
        private readonly IDistributorSubscriptionService _distributorSubscription;

        public DashboardController(IUserService user, IDistributorSubscriptionService distributorSubscription)
        {
            _user = user;
            _distributorSubscription = distributorSubscription;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var distributorId = Convert.ToInt64(User.GetClaimValue(UserClaims.DistributorId));
            if (_distributorSubscription.GetCount(x => x.DistributorId == distributorId && x.IsActive && x.EndDate.Value.Date > DateTime.Now.Date) == 0)
                return RedirectToAction("NewSubscriptionActivation", "MyAccount", new { area = "Distributor" });

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@PharmacyDistributorId", SqlDbType.BigInt) { Value = distributorId },
                new SqlParameter("@isPharmacy", SqlDbType.Bit) { Value = false }
            };
            var model = _user.GetPharmacyDistributorUserDashboard(parameters.ToArray());
            return View(model);
        }

    }
}

