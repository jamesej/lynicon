using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Modules;
using Lynicon.Repositories;
using Lynicon.Routing;

namespace Lynicon
{
    /// <summary>
    /// Class which sets up the controllers and routes from this assembly's namespace as being in the Lynicon area
    /// </summary>
    public class LyniconAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// The name of the Lynicon area
        /// </summary>
        public override string AreaName
        {
            get
            {
                return "Lynicon";
            }
        }

        /// <summary>
        /// Register all routes (via modules)
        /// </summary>
        /// <param name="context">Area registration context</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            LyniconModuleManager.Instance.EnsureRoutes(context, "Lynicon.Modules");
        }
    }
}
