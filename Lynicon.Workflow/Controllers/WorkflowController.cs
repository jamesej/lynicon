using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using System.Web.UI;
using Lynicon.Collation;
using Lynicon.Routing;
using Lynicon.Map;
using Lynicon.Models;
using LM = Lynicon.Membership;
using Lynicon.Workflow.Models;
using Lynicon.Membership;
using System.Net;

namespace Lynicon.Workflow.Controllers
{
    public class WorkflowController : Controller
    {
        [Authorize(Roles = LM.User.EditorRole), HttpPost]
        public ActionResult GetLayer(string name)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "No appropriate authenticated user");
            var newLayer = LayerManager.Instance.GetNewUserLayer(wfUser, name);
            return Json(newLayer);
        }

        [Authorize(Roles = LM.User.EditorRole), HttpPost]
        public ActionResult SetCurrentLayer(int level)
        {
            var wfUser = LyniconSecurityManager.Current.User as IWorkflowUser;
            if (wfUser == null)
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "No appropriate authenticated user");
            LayerManager.Instance.SetUserLayer(wfUser, level);
            return Content("OK");
        }
    }
}
