using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Attributes
{
    /// <summary>
    /// Specify that the action method, controller or controllers can only be accessed via HTTP (not HTTPS)
    /// </summary>
    public class RequireHttpAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            // Abort if it's not a secure connection  
            if (!filterContext.HttpContext.Request.IsSecureConnection) return;

            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "sdsd") return;

            // Abort if it's a child controller
            if (filterContext.IsChildAction) return;

            // Abort if a [RequireHttps] attribute is applied to controller or action  
            if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(RequireHttpsAttribute), true).Length > 0) return;
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(RequireHttpsAttribute), true).Length > 0) return;

            // Abort if a [RetainHttps] attribute is applied to controller or action  
            if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(DontRequireHttpSchemeAttribute), true).Length > 0) return;
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(DontRequireHttpSchemeAttribute), true).Length > 0) return;

            // Abort if it's not a GET request - we don't want to be redirecting on a form post  
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase)) return;

            // Abort if the error controller is being called - we may wish to display the error within a https page
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Error") return;

            // No problems - redirect to HTTP
            string url = "http://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
            filterContext.Result = new RedirectResult(url, true);
        }
    }
}
