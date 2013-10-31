using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Lynicon.Base.Models;
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

            context.AddDataRoute<List<Comment>>("lyniconbasecomments",
                "Lynicon/Comments",
                new { controller = "Comment", action = "List" });
            
        }
    }
}
