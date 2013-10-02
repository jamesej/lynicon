using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Repositories;
using Lynicon.Routing;

namespace Lynicon.Base
{
    public class LyniconBaseAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Lynicon.Base";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var x = 1;
            // Get dynamically generated content
            //context.MapRoute("lynicondynamic",
            //    "Lynicon/Dynamic/{action}/{urlTail}",
            //    new { controller = "Dynamic" });
            //context.AddDataRoute<User>("lyniconuser",
            //    "Lynicon/User/{_id}",
            //    new { controller = "User", action = "Index" }
            //    );
            
        }
    }
}
