using Microsoft.AspNetCore.Mvc;
using RxFair.Areas.Admin.Controllers;
using RxFair.Controllers;
using MyAccountController = RxFair.Areas.Distributor.Controllers.MyAccountController;

namespace RxFair.Utility.Extension
{
    public static class UrlHelperExtensions
    {
        public static string EmailConfirmationLink(this IUrlHelper urlHelper, long userId, string code, string scheme)
        {
            return urlHelper.Action(action: nameof(AccountController.ConfirmEmail), controller: "Account", values: new { userId, code }, protocol: scheme);
        }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, long userId, string code, string scheme)
        {
            return urlHelper.Action(action: nameof(AccountController.ResetPassword), controller: "Account", values: new { userId, code }, protocol: scheme);
        }

        public static string AfterPharmacyEmailConfirm(this IUrlHelper urlHelper, long id, string scheme)
        {
            return urlHelper.Action(action: nameof(NewPharmacyController.Index), controller: "NewPharmacy", values: new { id }, protocol: scheme);
        }

        public static string AfterDistributorEmailConfirm(this IUrlHelper urlHelper, long id, string scheme)
        {
            return urlHelper.Action(action: nameof(NewDistributorController.Index), controller: "NewDistributor", values: new { id }, protocol: scheme);
        }

        public static string NewSubscriptionActivation(this IUrlHelper urlHelper, long id, string code, string scheme)
        {
            return urlHelper.Action(action: nameof(MyAccountController.NewSubscriptionActivation), controller: "MyAccount", values: new { id, code }, protocol: scheme);
        }
    }
}
