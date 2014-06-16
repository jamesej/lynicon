using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Editors;
using Lynicon.Extensibility;
using Lynicon.Membership;
using Lynicon.Repositories;
using Lynicon.Routing;

namespace Lynicon.Workflow
{
    public class LyniconBaseAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Lynicon.Workflow";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            LyniconModuleManager.Instance.EnsureRoutes<WorkflowModule>(context);
        }
    }
}
