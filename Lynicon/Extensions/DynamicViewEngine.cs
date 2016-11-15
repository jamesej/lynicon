using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lynicon.Extensions
{
    /// <summary>
    /// A custom view engine which looks in a Views folder whose name is in the RouteData.DataTokens["viewdir"]
    /// </summary>
    public class DynamicViewEngine : RazorViewEngine
    {
        public DynamicViewEngine()
            : base()
        {

            AreaViewLocationFormats = new[] {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            AreaMasterLocationFormats = new[] {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            AreaPartialViewLocationFormats = new[] {
                "~/Areas/{2}/Views/%1/{1}/{0}.cshtml",
                "~/Areas/{2}/Views/%1/{1}/{0}.vbhtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.cshtml",
                "~/Areas/{2}/Views/%1/Shared/{0}.vbhtml"
            };

            ViewLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };

            MasterLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };

            PartialViewLocationFormats = new[] {
                "~/Views/%1/{1}/{0}.cshtml",
                "~/Views/%1/{1}/{0}.vbhtml",
                "~/Views/%1/Shared/{0}.cshtml",
                "~/Views/%1/Shared/{0}.vbhtml"
            };

        }

        /// <summary>
        /// Get the name of the view directory to search in
        /// </summary>
        /// <param name="controllerContext">Current ControllerContext</param>
        /// <returns>The name of the view directory</returns>
        private string GetViewDir(ControllerContext controllerContext)
        {
            return controllerContext.RouteData.DataTokens["viewdir"] as string;
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string viewDir = GetViewDir(controllerContext);
            if (viewDir == null)
                return null;
            else
                return base.CreatePartialView(controllerContext, partialPath.Replace("%1", viewDir));
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string viewDir = GetViewDir(controllerContext);
            if (viewDir == null)
                return null;
            else
                return base.CreateView(controllerContext,
                    viewPath.Replace("%1", viewDir),
                    masterPath.Replace("%1", viewDir));
        }

        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            string viewDir = GetViewDir(controllerContext);
            if (viewDir == null)
                return false;
            else
                return base.FileExists(controllerContext, virtualPath.Replace("%1", viewDir));
        }

    }
}
