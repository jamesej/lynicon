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
using System.Diagnostics;

namespace Lynicon.Editors
{
    /// <summary>
    /// Controller for showing the Dual Frame editor, the standard content editor which shows the resulting page in an iframe
    /// </summary>
    [Authorize(Roles = Membership.User.EditorRole)]
    public class DualFrameEditorController : EditorController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Show the dual frame editor
        /// </summary>
        /// <param name="data">content data to show in the editor</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(object data)
        {
            SetViewBag(data, updatedCheck: null);
            return View("LyniconDual", data);
        }

        /// <summary>
        /// Update data from the dual frame editor
        /// </summary>
        /// <param name="data">The original content data before changes</param>
        /// <param name="form">The form data representing the new content data</param>
        /// <param name="editAction">Perform a specific action to the content data if set</param>
        /// <param name="formState">Saved form state information</param>
        /// <param name="updatedCheck">Record of the updated date of the data when it was put into the user's form (in ticks)</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Index(object data, FormCollection form, string editAction, string formState, long? updatedCheck)
        {
            try
            {
                // updates the unchanged data with the changes from the form
                base.Bind(data, form);

                if (editAction != null)
                    DoEditAction(data, editAction);

                SetViewBag(data, updatedCheck);

                if (ModelState.IsValid)
                {
                    var redirectUrl = base.SaveWithRedirect(data);

                    if (ModelState.IsValid)
                    {
                        // set UpdatedCheck if its an auditable item
                        var container = Collator.Instance.GetContainer(data);
                        if (container is IBasicAuditable)
                            ViewBag.UpdatedCheck = ((IBasicAuditable)container).Updated.Ticks;

                        // we now know there were no errors, so we do this to allow any changes made to 'data' to be shown on the form 
                        ModelState.Clear();

                        if (redirectUrl != null)
                            return Redirect("/" + redirectUrl);

                        // post, redirect, get pattern to avoid resubmits
                        return Redirect(Request.RawUrl);
                    }
                }
                else
                {
                    var errs = ModelState.SelectMany(kvp => kvp.Value.Errors.Select(e => new { kvp.Key, err = e })).ToList();
                    Debug.WriteLine("Form errors: " + errs.Select(e => e.Key + ": " + (string.IsNullOrEmpty(e.err.ErrorMessage) ? e.err.Exception.Message : e.err.ErrorMessage)).Join("; "));
                }
            }
            catch (Exception ex)
            {
                log.Error("Save edit failed", ex);
            }

            return View("LyniconDual", data);
        }

        /// <summary>
        /// Operation to delete a content item
        /// </summary>
        /// <param name="data">Data of content item to be deleted</param>
        /// <returns>Status of operation</returns>
        [HttpPost, Authorize(Roles = Membership.User.AdminRole)]
        public ActionResult Delete(object data)
        {
            Type type = data.GetType();
            Collator.Instance.Delete(new Address(type, RouteData.GetOriginal()), data);
            return Content("OK");
        }

        // T-SQL datetime has resolution of roughly 10 milliseconds i.e. 100000 ticks
        private const long StorageResolutionTicks = 100000;

        /// <summary>
        /// Set up the form metadata for the editor
        /// </summary>
        /// <param name="data">The content data</param>
        /// <param name="updatedCheck">The updated timestamp in ticks of the when the original values in the data were got from the data api</param>
        protected void SetViewBag(object data, long? updatedCheck)
        {
            base.SetViewBag();

            ViewBag.UpdatedCheck = (long)(updatedCheck ?? long.MaxValue);
            ViewBag.ClashUser = "";
            var container = Collator.Instance.GetContainer(new Address(data.GetType(), RouteData.GetOriginal()), data);
            if (container is IBasicAuditable)
            {
                var aud = (IBasicAuditable)container;
                if (updatedCheck == null || RouteData.DataTokens.ContainsKey("LynNewItem"))
                    ViewBag.UpdatedCheck = aud.Updated.Ticks;
                else if (((updatedCheck.Value / StorageResolutionTicks) + 1) * StorageResolutionTicks < aud.Updated.Ticks)
                {
                    string userName = null;
                    if (aud.UserUpdated != null)
                    {
                        var clashUser = Collator.Instance.Get<User>(new ItemId(typeof(User), aud.UserUpdated));
                        if (clashUser != null)
                            userName = clashUser.UserName;
                    }

                    ModelState.AddModelError("updateClash",
                        string.Format("This item has been edited since you opened it by {0}. If you save it again, it will overwrite their changes. Refresh the page to get their changes but lose yours.",
                            (userName == null ? "someone we can't identify" : userName)));
                    // means next save will succeed
                    ViewBag.UpdatedCheck = long.MaxValue;
                }
            }

            string id = Collator.Instance.GetIdProperty(container.GetType()).GetValue(container).ToString();
            ViewBag.Id = id;
            string url = Request.RawUrl.UpTo("?");
            if (Request.QueryString.AllKeys.Contains("$type"))
            {
                url += "?$type=" + Request.QueryString["$type"];
            }
            ViewBag.Url = url;
        }

        /// <summary>
        /// Current state view to show for a page which does not yet exist in the CMS
        /// </summary>
        /// <returns>The view markup</returns>
        [HttpGet]
        public ActionResult Empty()
        {
            return View(ConfigHelper.GetViewPath("LyniconEmptyUrl.cshtml"));
        }

    }
}
