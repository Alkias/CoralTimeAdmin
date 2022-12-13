using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoralTimeAdmin.Infrastructure;
using Microsoft.Extensions.Logging;

namespace CoralTimeAdmin
{
    public class ErrorHandling : HandleErrorAttribute
    {
        private readonly ILogger<ErrorHandling> _logger = EngineContext.Current.Resolve<ILogger<ErrorHandling>>();

        public override void OnException(ExceptionContext filterContext) {

            string action = filterContext.RouteData.Values["action"].ToString();
            string controller = filterContext.RouteData.Values["controller"].ToString();

            filterContext.ExceptionHandled = true;
            var model = new HandleErrorInfo(filterContext.Exception, controller, action);

            _logger.LogError(444, filterContext.Exception, "ErrorHandling");

            filterContext.Result = new ViewResult()
            {
                ViewName = "~/Views/Error.cshtml",
                ViewData = new ViewDataDictionary(model)
            };
        }
    }
}