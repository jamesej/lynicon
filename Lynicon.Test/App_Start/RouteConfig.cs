using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lynicon.Base.Models;
using Lynicon.Routing;
using Lynicon.Test.Models;
using Lynicon.Utility;

namespace Lynicon.Test
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.AddDataRoute<TestContent>("test", "Test/{action}",
                new { controller = "Test", action = "Index" });

            routes.AddDataRoute<HeaderContent>("header", "Articles/{_0}",
                new { controller = "Header", action = "Index" });

            routes.AddDataRoute<ItemContent>("item", "Articles/{_0}/{_1}",
                new { controller = "Item", action = "Index" });

            routes.AddDataRoute<AltItemContent>("altitem", "Articles/{_0}/{_1}",
                new { controller = "AltItem", action = "Index" });

            routes.AddDataRoute<BlogPostContent>("blogpost", "Blog/{_0}/{_1}/{_2}/{_3}",
                new { controller = "BlogPost", action = "Index" });

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}