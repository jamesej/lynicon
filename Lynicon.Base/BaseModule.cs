using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Base.Models;
using Lynicon.Extensibility;
using Lynicon.Routing;

namespace Lynicon.Base
{
    public class BaseModule : Module
    {
        public BaseModule(AreaRegistrationContext context, params string[] dependentOn)
            : base("Base", context, dependentOn)
        {
        }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            regContext.AddDataRoute<List<Comment>>("lyniconbasecomments",
                "Lynicon/Comments",
                new { controller = "Comment", action = "List" });
            return true;
        }
    }
}
