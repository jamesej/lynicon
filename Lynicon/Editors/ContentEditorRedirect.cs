using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Lynicon.Membership;
using Lynicon.Routing;

namespace Lynicon.Editors
{
    /// <summary>
    /// Decide whether to reroute a request to an editor for Content persistence model
    /// </summary>
    public class ContentEditorRedirect : IEditorRedirect
    {
        #region IEditorRedirect Members

        public bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T: class, new()
        {
            string modeFlag = httpContext == null ? "" : (httpContext.Request.QueryString["$mode"] ?? "").ToLower();
            bool redirect =
                (LyniconSecurityManager.Current.CurrentUserInRole("E") && (modeFlag != "view" || data == null) && modeFlag != "bypass")
                || modeFlag == "ping";
            if (redirect)
            {
                string action = "Index";
                switch (modeFlag)
                {
                    case "editor":
                        action = "Editor";
                        break;
                    case "view": // only get here when viewing an unused url
                        action = "Empty";
                        break;
                    case "property-item-html":
                        action = "PropertyItemHtml";
                        break;
                    case "delete":
                        action = "Delete";
                        break;
                    case "ping":
                        action = "Ping";
                        break;
                }

                rd.RedirectAction("Lynicon", "DualFrameEditor", action);
                rd.RouteHandler = new MvcRouteHandler(); // we need to use standard mvc to show the editor

                if (data == null)
                {
                    data = new T();
                    rd.DataTokens.Add("LynNewItem", true);
                }
            }
            return redirect;
        }

        #endregion
    }
}
