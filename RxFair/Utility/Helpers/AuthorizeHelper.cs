using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using RxFair.Dto.Dtos;
using RxFair.Dto.Enum;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility.Extension;

namespace RxFair.Utility.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeHelper : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // Create SystemModule Services
            var systemModule = filterContext.HttpContext.RequestServices.GetRequiredService<ISystemModuleService>();

            var controllerInfo = filterContext.ActionDescriptor as ControllerActionDescriptor;
            {
                if (controllerInfo == null) return;
                var controllerActionModel = new ControllerActionModel
                {
                    Controller = controllerInfo.ControllerName,
                    Action = controllerInfo.ActionName
                };
                var userId = filterContext.HttpContext.User.GetUserId();
                var allowedPageList = GetUserAllowedMenu(filterContext.HttpContext, systemModule, userId);
                if (allowedPageList == null) return;

                var allowedControllerActionList = allowedPageList.Where(x => !string.IsNullOrEmpty(x.MenuDisplayText))
                    .Select(x => new ControllerActionModel
                    {
                        Controller = x.Controller.NullToString().ToLower(),
                        Action = x.Action.NullToString().ToLower()
                    }).ToList();
                var result = allowedControllerActionList.Any(x => x.Action.Equals(controllerActionModel.Action)
                                                               && x.Controller.Equals(controllerActionModel.Controller));

                if (result) return;
                if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new JsonResponse().GenerateJsonResult(0);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "Controller", "Account" }, { "Action", "AccessDenied" }
                    });
                }
            }
        }

        public static List<AllowedMenuViewModel> GetUserAllowedMenu(HttpContext context, ISystemModuleService systemModule, long userId)
        {
            var allowedPageList = context.Session.GetObjectFromJson<List<AllowedMenuViewModel>>("allowedPageList");
            if (allowedPageList != null) return allowedPageList;
            allowedPageList = SetUserAllowedMenu(systemModule, userId);
            context.Session.SetObjectAsJson("allowedPageList", allowedPageList);
            return allowedPageList;
        }

        public static List<AllowedMenuViewModel> SetUserAllowedMenu(ISystemModuleService systemModule, long userId)
        {
            if (userId == 0) return new List<AllowedMenuViewModel>();
            var param = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };
            return systemModule.GetUserAllowedMenu(param.ToArray());
        }
    }
    
    public class ControllerActionModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
