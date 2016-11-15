using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lynicon.Collation;
using Lynicon.Config;
using Lynicon.Utility;
using Lynicon.Routing;
using System.Web.Routing;
using Lynicon.Map;
using Lynicon.Extensibility;
using Lynicon.Models;
using Lynicon.Repositories;
using Lynicon.Membership;

namespace Lynicon.Editors
{
    /// <summary>
    /// An editor controller for HTTP webservice editing which sends JSON and updates data with standard form data
    /// </summary>
    public class WebEditorController : EditorController
    {
        private WebEditorRedirect GetRedirect(Type t)
        {
            return (WebEditorRedirect)RouteData.DataTokens["Redirect"];
        }

        /// <summary>
        /// Returns JSON for the content data
        /// </summary>
        /// <param name="data">The content data to show as JSON</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(object data)
        {
            if (!LyniconSecurityManager.Current.CurrentUserInRole(GetRedirect(data.GetType()).ReadRole))
                return new HttpStatusCodeResult(401);

            if (RouteData.DataTokens.ContainsKey("@Paging"))
            {
                var paging = (PagingSpec)RouteData.DataTokens["@Paging"];
                return Json(new { lynPaging = paging, lynData = data }, JsonRequestBehavior.AllowGet);
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates or adds content item data using standard form data
        /// </summary>
        /// <param name="data">The current content item (or items)</param>
        /// <param name="form">The form data to update / add with</param>
        /// <param name="editAction">Specific editing action</param>
        /// <returns>The status of the operation</returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(object data, FormCollection form, string editAction)
        {
            WebEditorRedirect redir = GetRedirect(data.GetType());

            if (!LyniconSecurityManager.Current.CurrentUserInRole(redir.WriteRole))
                return new HttpStatusCodeResult(401);
            if (redir.VerifySetPermissions != null &&
                !redir.VerifySetPermissions(ControllerContext, data, form))
                return new HttpStatusCodeResult(401);

            base.Bind(data, form);

            if (editAction != null)
                DoEditAction(data, editAction);

            if (ModelState.IsValid)
                base.Save(data);

            if (ModelState.IsValid)
                return Content("OK");
            else
                return Content("Failed");
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="data">The data item to delete</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(object data)
        {
            Type type = data.GetType();
            if (!LyniconSecurityManager.Current.CurrentUserInRole(GetRedirect(type).DeleteRole))
                return new HttpStatusCodeResult(401);

            try
            {
                Collator.Instance.Delete(new Address(type, RouteData.GetOriginal()), data);
            }
            catch
            {
                return Content("Failed");
            }

            return Content("OK");
        }
    }
}
