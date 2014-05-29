using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Base.Models;
using Lynicon.Collation;
using Lynicon.Extensibility;
using Lynicon.Repositories;
using Lynicon.Routing;
using Lynicon.Tasks.Models;

namespace Lynicon.Base
{
    public class TasksModule : Module
    {
        public TasksModule(AreaRegistrationContext context, params string[] dependentOn)
            : base("Tasks", context, dependentOn)
        {
            Collator.Instance.SetupType(typeof(ItemTask), new BasicCollator(), new BasicRepository());
        }

        public override bool Initialise(AreaRegistrationContext regContext)
        {
            if (!VerifyDbState("LyniconTasks 0.0"))
                return false;

            //Collator.Instance.Get<
            //regContext.AddDataRoute<List<Comment>>("lyniconbasecomments",
            //    "Lynicon/Comments",
            //    new { controller = "Comment", action = "List" });


            return true;
        }
    }
}
