using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongKe.Data.Models;
using ThongKe.Helps;

namespace ThongKe.Controllers
{
    public class BaseController: Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // var session = (user)Session[CommonConstants.USER_SESSION];
            // var username = HttpContext.Session.GetString("username");
            var user = HttpContext.Session.Get<Users>("loginUser");
            
            //var user = JsonConvert.DeserializeObject<Users>(HttpContext.Session.GetString("userInfo"));

            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", area = "" }));
            }
            base.OnActionExecuting(filterContext);
        }
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
    }
}
