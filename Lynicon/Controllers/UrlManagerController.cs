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
using Lynicon.Extensibility;
using Lynicon.Modules;

namespace Lynicon.Controllers
{
    /// <summary>
    /// Services to analyse urls in various ways
    /// </summary>
    public class UrlManagerController : Controller
    {
        public ActionResult Index()
        {
            return Content((string)this.RouteData.DataTokens["$urlget"], "text/plain");
        }

        /// <summary>
        /// Get the possible patterns for a new url of the given type
        /// </summary>
        /// <param name="datatype">Type as its name as a string</param>
        /// <returns>The patterns as JSON</returns>
        public ActionResult TypePatterns(string datatype)
        {
            Type type = ContentTypeHierarchy.GetContentType(datatype);
            var patterns = ContentMap.Instance.GetUrlPatterns(type);
            return Json(patterns, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Move the item whose id is in the query value of the query key $urlset and whose type is given to
        /// the current main url
        /// </summary>
        /// <param name="type">Type of the item to move</param>
        /// <returns>OK when done</returns>
        [HttpPost, Authorize(Roles = Lynicon.Membership.User.EditorRole)]
        public ActionResult MoveUrl(Type type)
        {
            try
            {
                Collator.Instance.MoveAddress(
                    new ItemId(type, (string)this.RouteData.DataTokens["$urlset"]),
                    new Address(type, this.RouteData.GetOriginal()));
            }
            catch (ApplicationException appEx)
            {
                if (appEx.Message == "There is an item already at that address")
                    return Content("Already Exists");
            }
            return Content("OK");
        }

        /// <summary>
        /// Simply returns the message Already Exists (as the result of a divert)
        /// </summary>
        /// <returns>The string "Already Exists"</returns>
        public ActionResult AlreadyExists()
        {
            return Content("Already Exists");
        }

        /// <summary>
        /// Delete a content item
        /// </summary>
        /// <param name="data">the content item to delete</param>
        /// <returns>OK when done</returns>
        [HttpPost, Authorize(Roles = Lynicon.Membership.User.AdminRole)]
        public ActionResult Delete(object data)
        {
            Collator.Instance.Delete(data);
            return Content("OK");
        }

        /// <summary>
        /// Simply returns the message Exists (as the result of a divert)
        /// </summary>
        /// <returns>The string "Exists"</returns>
        public ActionResult VerifyExists()
        {
            return Content("Exists");
        }

    }
}
