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
    /// <summary>
    /// General UI webservices
    /// </summary>
    public class UiController : Controller
    {
        /// <summary>
        /// Show the function panel reveal panel
        /// </summary>
        /// <returns>Markup for the reveal panel</returns>
        public ActionResult FunctionReveal()
        {
            return PartialView();
        }
    }
}
