﻿using System;
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
            // Register areas including area-modules
            AreaRegistration.RegisterAllAreas(this);

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Initialise caches
            LyniconModuleManager.Instance.RegisterModule(new SummaryCache(null));

            // We have all the modules, initialise them
            LyniconModuleManager.Instance.Initialise();

            DalTrack.Instance.Initialise();
            NavManager.Instance.RegisterNavManager(new UrlTreeNavManager<WikiContent>());
        }

        protected void Application_OnPostAuthenticateRequest(object sender, EventArgs e)
        {
            LyniconSecurityManager.Current.EnsureLightweightIdentity();
        }
    }
}