using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Extensibility;
using Lynicon.Map;
using Lynicon.Membership;
using Lynicon.Test.Models;
using Lynicon.Base.Models;

namespace Lynicon.Test
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LyniconConfig.RegisterModules();
            // Register areas
            AreaRegistration.RegisterAllAreas(this);

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            LyniconConfig.Initialise();
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            LyniconSecurityManager.Current.EnsureLightweightIdentity();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            var x = HttpContext.Current;
            var y = HttpContext.Current.ApplicationInstance;
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            var x = HttpContext.Current;
            var y = HttpContext.Current.ApplicationInstance;
        }
    }
}