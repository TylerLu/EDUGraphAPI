using System.Web.Mvc;

namespace EDUGraphAPI.Web.Infrastructure
{
    /// <summary>
    /// Allow the web app to redirect the current user to the proper login page in our multi-authentication-method scenario
    /// </summary>
    public class EduAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
                filterContext.Result = new JsonResult { Data = new { error = "Unauthorized" } };
            else
            {
                filterContext.Result = new RedirectResult("/Account/Login");

            }
        }
    }
}