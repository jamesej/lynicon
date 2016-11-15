using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Lynicon.Attributes;
using Lynicon.Membership;
using Lynicon.Routing;
using Lynicon.Utility;

namespace Lynicon.Editors
{
    /// <summary>
    /// Redirect for the list/detail editor
    /// </summary>
    public class ListEditorRedirect : IEditorRedirect
    {

        string view = "LyniconListDetail";
        List<string> rowFields = null;

        /// <summary>
        /// Create a list editor redirect for default view and row fields
        /// </summary>
        public ListEditorRedirect()
        { }
        /// <summary>
        /// Create a list editor redirect specifying the view to use
        /// </summary>
        /// <param name="view">View name for the editor</param>
        public ListEditorRedirect(string view)
            : this(view, null)
        { }
        /// <summary>
        /// Create a list editor redirect specifying which fields to show on each row
        /// </summary>
        /// <param name="rowFields">List of the names of the fields to show on each row</param>
        public ListEditorRedirect(List<string> rowFields)
            : this(null, rowFields)
        { }
        /// <summary>
        /// Create a list editor redirect specifying the view to use and which fields to show on each row
        /// </summary>
        /// <param name="view">View name for the editor</param>
        /// <param name="rowFields">List of the names of the fields to show on each row</param>
        public ListEditorRedirect(string view, List<string> rowFields)
        {
            this.view = view;
            this.rowFields = rowFields;
        }


        #region IEditorRedirect Members

        /// <summary>
        /// Decide whether to redirect to the editor for the list/detail editor
        /// </summary>
        /// <typeparam name="T">The type of the list items</typeparam>
        /// <param name="rd">the route data for the request</param>
        /// <param name="httpContext">the http context for the request</param>
        /// <param name="data">the original list of items</param>
        /// <returns>Whether to redirect</returns>
        public bool Redirect<T>(RouteData rd, HttpContextBase httpContext, object data) where T : class, new()
        {
            string view = this.view;
            string modeFlag = httpContext == null ? "" : (httpContext.Request.QueryString["$mode"] ?? "").ToLower();
            bool redirect =
                (LyniconSecurityManager.Current.CurrentUserInRole("E") && modeFlag != "view")
                || modeFlag == "ping";
            if (redirect)
            {
                string action = "Index";
                switch (modeFlag)
                {
                    case "getvalues":
                        action = "GetValues";
                        break;
                    case "delete":
                        action = "Delete";
                        break;
                    case "property-item-html":
                        action = "PropertyItemHtml";
                        break;
                    case "ping":
                        action = "Ping";
                        break;
                }

                rd.RedirectAction("Lynicon", "ListEditor", action);
                rd.RouteHandler = new MvcRouteHandler(); // we need to use standard mvc to show the editor

                if (!typeof(T).IsGenericType)
                    throw new ArgumentException("ListEditorRedirect called with non list type " + typeof(T).FullName);

                Type itemType = typeof(T).GetGenericArguments().First();
                ListEditorAttribute attr = itemType.GetCustomAttribute<ListEditorAttribute>();
                if (attr != null)
                    view = attr.ViewName;

                if (!rd.Values.ContainsKey("view"))
                    rd.Values["view"] = view;

                if (!rd.Values.ContainsKey("listView"))
                    rd.Values["listView"] = "ObjectList";

                if (!rd.Values.ContainsKey("rowFields"))
                {
                    if (rowFields == null)
                        rd.Values["rowFields"] = itemType
                            .GetPersistedProperties()
                            .Select(pi => pi.Name)
                            .Except(new string[] { "Id" })
                            .Join(",");
                    else
                        rd.Values["rowFields"] = rowFields.Join(",");
                }
                    
            }
            return redirect;
        }

        #endregion
    }
}
