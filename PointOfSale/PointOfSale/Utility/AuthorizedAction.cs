using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using POS.DataAccessLayer.Models.Security;

namespace Admin.Components
{
    public class AuthorizedAction : ActionFilterAttribute
    {
        private UserManager<User> _userManager;

        public AuthorizedAction(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //var isAjax = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            //if (isAjax)
            //    return;

            var user = _userManager.GetUserAsync(filterContext.HttpContext.User).Result;

            if (user == null || user.CompanyId == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                return;
            }
            return;
        }
    }
}
