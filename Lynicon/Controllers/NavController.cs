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

namespace Lynicon.Controllers
{
    public class NavController : Controller
    {
        public ActionResult Children(string idString)
        {
            object id;
            Guid g;
            int i;
            if (Guid.TryParse(idString, out g))
                id = Guid.Parse(idString);
            else if (int.TryParse(idString, out i))
                id = int.Parse(idString);
            else
                id = idString;
            var children = NavManager.Instance.GetChildren(id);
            return View("NavList", children);
        }
    }
}
